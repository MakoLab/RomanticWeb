using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace RomanticWeb.dotNetRDF.TripleSources
{
    class InMemoryApiStrategy : IStoreQueryStrategy
    {
        private static readonly NodeFactory NodeFactory = new NodeFactory();
        private readonly IInMemoryQueryableStore _tripleStore;

        public InMemoryApiStrategy(IInMemoryQueryableStore tripleStore)
        {
            _tripleStore = tripleStore;
        }

        public IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate)
        {
            INode entityNode = entityId.ToNode(NodeFactory);
            INode predicateNode = NodeFactory.CreateUriNode(predicate.Uri);

            return _tripleStore.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                               .Select(t => t.Object.WrapNode());
        }

        public bool TryGetListElements(RdfNode rdfList, out IEnumerable<RdfNode> listElements)
        {
            IGraph graph = _tripleStore[rdfList.GraphUri];
            IBlankNode listNode = graph.GetBlankNode(rdfList.BlankNodeId);
            IUriNode rdfFirst = graph.CreateUriNode(UriFactory.Create(RdfSpecsHelper.RdfListFirst));

            if (graph.GetTriplesWithSubjectPredicate(listNode, rdfFirst).Any())
            {
                listElements = graph.GetListItems(listNode).Select(n => n.WrapNode()).ToList();
                return true;
            }

            listElements = null;
            return false;
        }
    }
}