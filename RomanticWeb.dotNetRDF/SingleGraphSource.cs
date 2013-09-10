using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class SingleGraphSource : TriplesSourceBase
    {
        private readonly IGraph _graph;

        public SingleGraphSource(IGraph graph)
        {
            _graph = graph;
        }

        public override IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate)
        {
            INode entityNode = _graph.CreateUriNode(entityId.Uri);
            INode predicateNode = _graph.CreateUriNode(predicate.Uri);

            return _graph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                         .Select(t => WrapObjectNode(t.Object));
        }
    }
}