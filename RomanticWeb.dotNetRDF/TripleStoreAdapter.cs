using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Sparql;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>An implementation of <see cref="IEntitySource"/>, which reads triples from a VDS.RDF.ITripleStore.</summary>
    public class TripleStoreAdapter : IEntitySource
    {
        private readonly ITripleStore _store;
        private readonly INamespaceMapper _namespaces;
        private readonly IEntityStore _entityStore;
        private readonly ISparqlCommandFactory _sparqlCommandFactory;

        /// <summary>
        /// Creates a new instance of <see cref="TripleStoreAdapter" />
        /// </summary>
        /// <param name="store">The underlying triple store</param>
        /// <param name="entityStore">The entity store.</param>
        /// <param name="sparqlCommandFactory"></param>
        public TripleStoreAdapter(ITripleStore store, IEntityStore entityStore, ISparqlCommandFactory sparqlCommandFactory)
        {
            _store = store;
            _entityStore = entityStore;
            _sparqlCommandFactory = sparqlCommandFactory;
            _namespaces = new NamespaceMapper(true);
            _namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
        }

        /// <summary>Uri of the meta graph, which contains information about Entities' named graphs.</summary>
        public Uri MetaGraphUri { get; set; }

        /// <summary>Loads an entity using SPARQL query and loads the resulting triples to the <paramref name="store"/>.</summary>
        public void LoadEntity(IEntityStore store, EntityId entityId)
        {
            // todo: maybe this should return EntityTriples instead and they should be asserted in EntityContext?
            var sparql = QueryBuilder.Select("s", "p", "o", "g")
                                   .Graph("?g", graph => graph.Where(triple => triple.Subject("s").Predicate("p").Object("o")))
                                   .Where(triple => triple.Subject("g").PredicateUri("foaf:primaryTopic").Object(entityId.Uri));
            sparql.Prefixes.Import(_namespaces);
            var triples = from result in ExecuteSelect(sparql.BuildQuery())
                          let subject = result["s"].WrapNode(entityId)
                          let predicate = result["p"].WrapNode(entityId)
                          let @object = result["o"].WrapNode(entityId)
                          let graph = result.HasBoundValue("g") ? result["g"].WrapNode(entityId) : null
                          select new EntityQuad(entityId, subject, predicate, @object, graph);

            _entityStore.AssertEntity(entityId, triples);
        }

        /// <summary>Executes an ASK query to perform existence check.</summary>
        public bool EntityExist(EntityId entityId)
        {
            var ask = QueryBuilder.Ask()
                                .Graph(
                                    MetaGraphUri,
                                    graph => graph.Where(triple => triple.Subject("g").PredicateUri("foaf:primaryTopic").Object(entityId.Uri)));
            ask.Prefixes.Import(_namespaces);
            return ExecuteAsk(ask.BuildQuery());
        }

        /// <summary>Executes a SPARQL query and returns resulting quads</summary>
        /// <param name="queryModel">Query model to be executed.</param>
        /// <returns>Enumeration of entity quads beeing a result of the query.</returns>
        public IEnumerable<EntityQuad> ExecuteEntityQuery(Query queryModel)
        {
            SparqlQueryVariables variables;
            var resultSet = ExecuteSelect(GetSparqlQuery(queryModel, out variables));
            return from result in resultSet
                   let id = (result[variables.Entity] is IBlankNode ?
                    new BlankId(((IBlankNode)result[variables.Entity]).InternalID, null, ((IUriNode)result[variables.MetaGraph]).Uri) :
                    new EntityId(((IUriNode)result[variables.Entity]).Uri))
                   let s = result[variables.Subject].WrapNode(id)
                   let p = result[variables.Predicate].WrapNode(id)
                   let o = result[variables.Object].WrapNode(id)
                   let g = result[variables.MetaGraph].WrapNode(id)
                   select new EntityQuad(id, s, p, o, g);
        }

        /// <summary>Executes a SPARQL query with scalar result.</summary>
        /// <param name="queryModel">Query model to be executed.</param>
        /// <returns>Scalar value beeing a result of the query.</returns>
        public int ExecuteScalarQuery(Query queryModel)
        {
            SparqlQueryVariables variables;
            var resultSet = ExecuteSelect(GetSparqlQuery(queryModel, out variables));
            return (from result in resultSet select Int32.Parse(((ILiteralNode)result[variables.Scalar]).Value)).FirstOrDefault();
        }

        /// <summary>Executes a SPARQL ask query.</summary>
        /// <param name="queryModel">Query model to be executed.</param>
        /// <returns><b>true</b> in case a given query has solution, otherwise <b>false</b>.</returns>
        public bool ExecuteAskQuery(Query queryModel)
        {
            return ExecuteSelect(GetSparqlQuery(queryModel)).Result;
        }

        /// <summary>One-by-one retracts deleted triples, asserts new triples and updates the meta graph.</summary>
        /// <param name="changes"></param>
        public void Commit(IEnumerable<DatasetChange> changes)
        {
            var updateCommands = changes.SelectMany(_sparqlCommandFactory.CreateCommands);
            var commands = new SparqlUpdateCommandSet(updateCommands);

            LogTo.Debug("Executing SPARQL Update:{0}{1}", Environment.NewLine, commands);
            ExecuteCommandSet(commands);
        }

        private SparqlQuery GetSparqlQuery(Query sparqlQuery)
        {
            SparqlQueryVariables variables;
            return GetSparqlQuery(sparqlQuery, out variables);
        }

        private SparqlQuery GetSparqlQuery(Query sparqlQuery, out SparqlQueryVariables variables)
        {
            GenericSparqlQueryVisitor queryVisitor = new SparqlQueryVisitor();
            queryVisitor.MetaGraphUri = MetaGraphUri;
            queryVisitor.VisitQuery(sparqlQuery);
            variables = queryVisitor.Variables;
            LogTo.Debug("Parsed query: {0}", queryVisitor.CommandText);
            SparqlQueryParser parser = new SparqlQueryParser();
            return parser.ParseFromString(queryVisitor.CommandText);
        }

        private void ExecuteCommandSet(SparqlUpdateCommandSet commands)
        {
            var store = _store as IUpdateableTripleStore;
            if (store == null)
            {
                throw new InvalidOperationException(string.Format("Store doesn't implement {0}", typeof(IUpdateableTripleStore)));
            }

            store.ExecuteUpdate(commands);
        }

        private bool ExecuteAsk(SparqlQuery query)
        {
            var store = _store as IInMemoryQueryableStore;
            if (store != null)
            {
                var inMemoryQuadDataset = new InMemoryQuadDataset(store, MetaGraphUri);
                var processor = new LeviathanQueryProcessor(inMemoryQuadDataset);
                return ((SparqlResultSet)processor.ProcessQuery(query)).Result;
            }

            return ((SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString())).Result;
        }

        private SparqlResultSet ExecuteSelect(SparqlQuery query)
        {
            var store = _store as IInMemoryQueryableStore;
            if (store != null)
            {
                var inMemoryQuadDataset = new InMemoryQuadDataset(store, MetaGraphUri);
                var processor = new LeviathanQueryProcessor(inMemoryQuadDataset);
                return (SparqlResultSet)processor.ProcessQuery(query);
            }

            return (SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString());
        }
    }
}