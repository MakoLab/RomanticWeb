using System;
using System.Collections.Generic;
using System.Linq;
using Resourcer;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Sparql;
using RomanticWeb.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>An implementation of <see cref="IEntitySource"/>, which reads triples from a VDS.RDF.ITripleStore.</summary>
    public class TripleStoreAdapter:IEntitySource
    {
        private readonly ITripleStore _store;
        private readonly INamespaceMapper _namespaces;
        private Uri _metaGraphUri=new Uri("http://app.magi/graphs");

        /// <summary>Creates a new instance of <see cref="TripleStoreAdapter"/></summary>
        /// <param name="store">The underlying triple store</param>
        public TripleStoreAdapter(ITripleStore store)
        {
            _store=store;
            _namespaces=new NamespaceMapper(true);
            _namespaces.AddNamespace("foaf",new Uri("http://xmlns.com/foaf/0.1/"));
        }

        /// <summary>Uri of the meta graph, which contains information about Entities' named graphs.</summary>
        public Uri MetaGraphUri
        {
            get
            {
                return _metaGraphUri;
            }

            set
            {
                _metaGraphUri=value;
            }
        }

        /// <summary>Loads an entity using SPARQL query and loads the resulting triples to the <paramref name="store"/>.</summary>
        public void LoadEntity(IEntityStore store,EntityId entityId)
        {
            // todo: maybe this should return EntityTriples instead and they should be asserted in EntityContext?
            var sparql=QueryBuilder.Select("s","p","o","g")
                                   .Graph("?g",graph => graph.Where(triple => triple.Subject("s").Predicate("p").Object("o")))
                                   .Where(triple => triple.Subject("g").PredicateUri("foaf:primaryTopic").Object(entityId.Uri));
            sparql.Prefixes.Import(_namespaces);
            var triples=from result in ExecuteSelect(sparql.BuildQuery())
                        let subject=result["s"].WrapNode()
                        let predicate=result["p"].WrapNode()
                        let @object=result["o"].WrapNode()
                        let graph=result.HasBoundValue("g")?result["g"].WrapNode():null
                        select new EntityQuad(entityId,subject,predicate,@object,graph);

            store.AssertEntity(entityId,triples);
        }

        /// <summary>Executes an ASK query to perform existence check.</summary>
        public bool EntityExist(EntityId entityId)
        {
            var ask=QueryBuilder.Ask()
                                .Graph(
                                    _metaGraphUri,
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
            var resultSet=ExecuteSelect(GetSparqlQuery(queryModel, out variables));
            return from result in resultSet
                   let id=new EntityId(((IUriNode)result[variables.Entity]).Uri)
                   let s = result[variables.Subject].WrapNode()
                   let p = result[variables.Predicate].WrapNode()
                   let o = result[variables.Object].WrapNode()
                   let g = result[variables.MetaGraph].WrapNode()
                   select new EntityQuad(id,s,p,o,g);
        }

        /// <summary>Executes a SPARQL query with scalar result.</summary>
        /// <param name="queryModel">Query model to be executed.</param>
        /// <returns>Scalar value beeing a result of the query.</returns>
        public int ExecuteScalarQuery(Query queryModel)
        {
            SparqlQueryVariables variables;
            var resultSet=ExecuteSelect(GetSparqlQuery(queryModel,out variables));
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
        public void ApplyChanges(DatasetChanges datasetChanges)
        {
            foreach (var triple in datasetChanges.QuadsRemoved)
            {
                var graph=GetGraph(triple.Graph.UnWrapGraphUri());
                graph.Retract(
                    triple.Subject.UnWrapNode(graph),triple.Predicate.UnWrapNode(graph),triple.Object.UnWrapNode(graph));
            }

            foreach (var triple in datasetChanges.QuadsAdded)
            {
                var graph=GetGraph(triple.Graph.UnWrapGraphUri());
                graph.Assert(
                    triple.Subject.UnWrapNode(graph),triple.Predicate.UnWrapNode(graph),triple.Object.UnWrapNode(graph));
            }

            // todo: more flexible delete
            ExecuteDelete(new SparqlUpdateCommandSet(GetDeleteCommands(datasetChanges.DeletedEntites)));

            // todo: find a way to allow users to extend the meta graph information
            var metaGraph=GetGraph(MetaGraphUri);
            var foafTopic=metaGraph.CreateUriNode(new Uri("http://xmlns.com/foaf/0.1/primaryTopic"));
            foreach (var metaGraphChange in datasetChanges.MetaGraphChanges)
            {
                metaGraph.Assert(
                    metaGraph.CreateUriNode(metaGraphChange.Item1),
                    foafTopic,
                    metaGraph.CreateUriNode(metaGraphChange.Item2.Uri));
            }
        }

        private IEnumerable<SparqlUpdateCommand> GetDeleteCommands(IEnumerable<EntityId> entitiesToDelete)
        {
            // todo: implement DELETE in Fluent SPARQL
            var parser=new SparqlUpdateParser();
            foreach (var entityId in entitiesToDelete)
            {
                var delete =new SparqlParameterizedString(Resource.AsString("Queries.DeleteEntity.ru"));
                delete.SetUri("entityId",entityId.Uri);
                delete.SetUri("metaGraph",MetaGraphUri);

                foreach (var command in parser.ParseFromString(delete).Commands)
                {
                    yield return command;
                }
            }
        }

        private SparqlQuery GetSparqlQuery(Query sparqlQuery)
        {
            SparqlQueryVariables variables;
            return GetSparqlQuery(sparqlQuery,out variables);
        }

        private SparqlQuery GetSparqlQuery(Query sparqlQuery, out SparqlQueryVariables variables)
        {
            GenericSparqlQueryVisitor queryVisitor=new GenericSparqlQueryVisitor();
            queryVisitor.MetaGraphUri=MetaGraphUri;
            queryVisitor.VisitQuery(sparqlQuery);
            variables=queryVisitor.Variables;
            SparqlQueryParser parser=new SparqlQueryParser();
            return parser.ParseFromString(queryVisitor.CommandText);
        }

        private IGraph GetGraph(Uri graphUri)
        {
            if (!_store.HasGraph(graphUri))
            {
                _store.Add(new Graph { BaseUri=graphUri });
            }

            return _store[graphUri];
        }

        private void ExecuteDelete(SparqlUpdateCommandSet deleteCommands)
        {
            var store=_store as IUpdateableTripleStore;
            if (store==null)
            {
                throw new InvalidOperationException(string.Format("Store doesn't implement {0}",typeof(IUpdateableTripleStore)));
            }

            store.ExecuteUpdate(deleteCommands);
        }

        private bool ExecuteAsk(SparqlQuery query)
        {
            var store=_store as IInMemoryQueryableStore;
            if (store!=null)
            {
                var inMemoryQuadDataset=new InMemoryQuadDataset(store,MetaGraphUri);
                var processor=new LeviathanQueryProcessor(inMemoryQuadDataset);
                return ((SparqlResultSet)processor.ProcessQuery(query)).Result;
            }

            return ((SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString())).Result;
        }

        private SparqlResultSet ExecuteSelect(SparqlQuery query)
        {
            var store=_store as IInMemoryQueryableStore;
            if (store!=null)
            {
                var inMemoryQuadDataset=new InMemoryQuadDataset(store,MetaGraphUri);
                var processor=new LeviathanQueryProcessor(inMemoryQuadDataset);
                return (SparqlResultSet)processor.ProcessQuery(query);
            }

            return (SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString());
        }
    }
}