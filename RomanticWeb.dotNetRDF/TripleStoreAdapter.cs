using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
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

        public IEnumerable<Tuple<Node,Node,Node>> GetNodesForQuery(string sparqlConstruct)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads an entity using SPARQL query and loads the resulting triples to the <paramref name="store"/>
        /// </summary>
        public void LoadEntity(IEntityStore store,EntityId entityId)
        {
            var select = QueryBuilder.Select("s", "p", "o", "g")
                                     .Graph("?g", g => g.Where(t => t.Subject("s").Predicate("p").Object("o")))
                                     .Where(t => t.Subject("g").PredicateUri("foaf:primaryTopic").Object(entityId.Uri));
            select.Prefixes.Import(_namespaces);

            foreach (var result in ExecuteEntityLoadQuery(select.BuildQuery()))
            {
                Node subject = result["s"].WrapNode();
                Node predicate = result["p"].WrapNode();
                Node @object=result["o"].WrapNode();
                Node graph=result.HasBoundValue("g")?result["g"].WrapNode():null;
                store.AssertTriple(new EntityTriple(entityId,subject,predicate,@object,graph));
            }
        }

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