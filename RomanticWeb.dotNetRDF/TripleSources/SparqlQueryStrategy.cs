using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Query;

namespace RomanticWeb.dotNetRDF.TripleSources
{
    class SparqlQueryStrategy : IStoreQueryStrategy
    {
        private readonly INativelyQueryableStore _nativeStore;
        private static readonly NodeFactory NodeFactory = new NodeFactory();

        public SparqlQueryStrategy(INativelyQueryableStore tripleStore)
        {
            _nativeStore = tripleStore;
        }

        public IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Uri predicate)
        {
            var query = new SparqlParameterizedString();
            query.CommandText = "SELECT ?o { GRAPH ?g { @entity @predicate ?o } }";
            query.SetParameter("entity", entityId.ToNode(NodeFactory));
            query.SetUri("predicate", predicate);

            return from SparqlResult result in (SparqlResultSet)_nativeStore.ExecuteQuery(query.ToString())
                   where result.HasBoundValue("o")
                   select result["o"].WrapNode();
        }

        public bool TryGetListElements(RdfNode rdfList, out IEnumerable<RdfNode> listElements)
        {
            throw new System.NotImplementedException();
        }
    }
}