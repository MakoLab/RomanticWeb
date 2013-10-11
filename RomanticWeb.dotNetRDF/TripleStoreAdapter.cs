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
    public class TripleStoreAdapter:IEntitySource
    {
        private readonly ITripleStore _store;

        private readonly INamespaceMapper _namespaces;

        public TripleStoreAdapter(ITripleStore store)
        {
            _store=store;
            _namespaces = new NamespaceMapper(true);
            _namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
        }

        public IEnumerable<Tuple<Node,Node,Node>> GetNodesForQuery(string sparqlConstruct)
        {
            throw new NotImplementedException();
        }

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

        private SparqlResultSet ExecuteEntityLoadQuery(SparqlQuery query)
        {
            var store=_store as IInMemoryQueryableStore;
            if (store!=null)
            {
                var inMemoryQuadDataset=new InMemoryQuadDataset(store,new Uri("http://app.magi/graphs"));
                var processor=new LeviathanQueryProcessor(inMemoryQuadDataset);
                return (SparqlResultSet)processor.ProcessQuery(query);
            }

            return (SparqlResultSet)((INativelyQueryableStore)_store).ExecuteQuery(query.ToString());
        }
    }
}