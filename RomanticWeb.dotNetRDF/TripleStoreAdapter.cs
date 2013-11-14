using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Model;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Datasets;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>
    /// An implementation of <see cref="IEntitySource"/>, which reads triples from a VDS.RDF.ITripleStore
    /// </summary>
    public class TripleStoreAdapter:IEntitySource
    {
        private readonly ITripleStore _store;

        private readonly INamespaceMapper _namespaces;

        private Uri _metaGraphUri=new Uri("http://app.magi/graphs");

        /// <summary>
        /// Creates a new instance of <see cref="TripleStoreAdapter"/>
        /// </summary>
        /// <param name="store">the underlying triple store</param>
        public TripleStoreAdapter(ITripleStore store)
        {
            _store=store;
            _namespaces = new NamespaceMapper(true);
            _namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
        }

        /// <summary>
        /// Uri of the meta graph, which contains information about Entities' named graphs
        /// </summary>
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

        /// <summary>
        /// Loads an entity using SPARQL query and loads the resulting triples to the <paramref name="store"/>
        /// </summary>
        public void LoadEntity(IEntityStore store,EntityId entityId)
        {
            // todo: maybe this should return EntityTriples instead and they should be asserted in EntityContext?
            var sparql = QueryBuilder.Select("s", "p", "o", "g")
                                     .Graph("?g", g => g.Where(t => t.Subject("s").Predicate("p").Object("o")))
                                     .Where(t => t.Subject("g").PredicateUri("foaf:primaryTopic").Object(entityId.Uri));
            sparql.Prefixes.Import(_namespaces);

            var triples=from result in ExecuteEntityLoadQuery(sparql.BuildQuery())
                        let subject = result["s"].WrapNode()
                        let predicate = result["p"].WrapNode()
                        let @object = result["o"].WrapNode()
                        let graph = result.HasBoundValue("g")?result["g"].WrapNode():null
                        select new EntityQuad(entityId,subject,predicate,@object,graph);

            store.AssertEntity(entityId,triples);
        }

        /// <summary>
        /// Executes an ASK query to perform existence check
        /// </summary>
        public bool EntityExist(EntityId entityId)
        {
            var ask=QueryBuilder.Ask()
                                .Graph(
                                    "?g",
                                    g => g.Where(t => t.Subject("s").Predicate("p").Object(entityId.Uri))
                                          .Union(u => u.Where(t=>t.Subject(entityId.Uri).Predicate("p").Object("o"))));
            ask.Prefixes.Import(_namespaces);

            return ExecuteAsk(ask.BuildQuery());
        }

        /// <inheritdoc />
        public IEnumerable<EntityQuad> ExecuteEntityQuery(QueryModel sparqlQuery)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// One-by-one retracts deleted triples, asserts new triples
        /// and updates the meta graph
        /// </summary>
        public void ApplyChanges(DatasetChanges datasetChanges)
        {
            foreach (var triple in datasetChanges.TriplesRemoved)
            {
                var graph=GetGraph(triple.Graph.UnWrapGraphUri());
                graph.Retract(
                    triple.Subject.UnWrapNode(graph),
                    triple.Predicate.UnWrapNode(graph),
                    triple.Object.UnWrapNode(graph));
            }

            foreach (var triple in datasetChanges.TriplesAdded)
            {
                var graph=GetGraph(triple.Graph.UnWrapGraphUri());
                graph.Assert(
                    triple.Subject.UnWrapNode(graph),
                    triple.Predicate.UnWrapNode(graph),
                    triple.Object.UnWrapNode(graph));
            }

            // todo: find a way to allow users to extend the meta graph information
            var metaGraph=GetGraph(MetaGraphUri);
            var foafTopic = metaGraph.CreateUriNode(new Uri("http://xmlns.com/foaf/0.1/primaryTopic"));
            foreach (var metaGraphChange in datasetChanges.MetaGraphChanges)
            {
                metaGraph.Assert(
                    metaGraph.CreateUriNode(metaGraphChange.Item1),
                    foafTopic,
                    metaGraph.CreateUriNode(metaGraphChange.Item2.Uri));
            }
        }

        private IGraph GetGraph(Uri graphUri)
        {
            if (!_store.HasGraph(graphUri))
            {
                _store.Add(new Graph { BaseUri=graphUri });
            }

            return _store[graphUri];
        }

        private bool ExecuteAsk(SparqlQuery query)
        {
            var store = _store as IInMemoryQueryableStore;
            if (store != null)
            {
                var inMemoryQuadDataset=new InMemoryQuadDataset(store,MetaGraphUri);
                var processor = new LeviathanQueryProcessor(inMemoryQuadDataset);
                return ((SparqlResultSet)processor.ProcessQuery(query)).Result;
            }

            return ((SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString())).Result;
        }

        private SparqlResultSet ExecuteEntityLoadQuery(SparqlQuery query)
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