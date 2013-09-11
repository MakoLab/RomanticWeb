using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF.TripleSources
{
    public class SingleGraphSource : ITriplesSource
    {
        private readonly IGraph _graph;

        public SingleGraphSource(IGraph graph)
        {
            _graph = graph;
        }

        public virtual IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate)
        {
            INode entityNode = entityId.ToNode(_graph);
            INode predicateNode = _graph.CreateUriNode(predicate.Uri);

            return _graph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                         .Select(t => t.Object.WrapNode());
        }

        public bool TryGetListElements(RdfNode rdfList, out IEnumerable<RdfNode> listElements)
        {
            throw new System.NotImplementedException();
        }
    }
}