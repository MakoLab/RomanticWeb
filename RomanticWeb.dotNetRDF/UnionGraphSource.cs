using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Query;

namespace RomanticWeb.dotNetRDF
{
    public class UnionGraphSource : TriplesSourceBase
    {
        private readonly ITripleStore _tripleStore;
        private static readonly NodeFactory NodeFactory = new NodeFactory();

        public UnionGraphSource(ITripleStore tripleStore)
        {
            _tripleStore = tripleStore;
        }

        public override IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate)
        {
            var queryableStore = _tripleStore as IInMemoryQueryableStore;
            if (queryableStore != null)
            {
                INode entityNode = NodeFactory.CreateUriNode(entityId.Uri);
                INode predicateNode = NodeFactory.CreateUriNode(predicate.Uri);

                return queryableStore.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                                     .Select(t => WrapObjectNode(t.Object));
            }

            var nativeStore = _tripleStore as INativelyQueryableStore;
            if (nativeStore != null)
            {
                var query = new SparqlParameterizedString();
                query.CommandText = "SELECT ?o { GRAPH ?g { @entity @predicate ?o } }";
                query.SetUri("entity", entityId.Uri);
                query.SetUri("predicate", predicate.Uri);

                return from SparqlResult result in (SparqlResultSet)nativeStore.ExecuteQuery(query.ToString())
                       where result.HasBoundValue("o")
                       select WrapObjectNode(result["o"]);
            }

            throw new InvalidOperationException("The triple store was neither IInMemoryQueryableStore nor INativelyQueryableStore and thus it cannot be queried");
        }
    }
}