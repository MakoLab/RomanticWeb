using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class SingleGraphSource : ITriplesSource
    {
        private readonly IGraph _graph;

        public SingleGraphSource(IGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate)
        {
            INode entityNode = _graph.CreateUriNode(entityId.Uri);
            INode predicateNode = _graph.CreateUriNode(predicate.Uri);

            return _graph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                         .Select(t => WrapObjectNode(t.Object));
        }

        private static RdfNode WrapObjectNode(INode arg)
        {
            var literal = arg as ILiteralNode;
            if (literal != null)
            {
                return RdfNode.ForLiteral(literal.Value, literal.Language, literal.DataType);
            }
            var uriNode = arg as IUriNode;
            if (uriNode != null)
            {
                return RdfNode.ForUri(uriNode.Uri);
            }

            throw new NotImplementedException("Blank nodes aren't supported yet");
        }   
    }
}