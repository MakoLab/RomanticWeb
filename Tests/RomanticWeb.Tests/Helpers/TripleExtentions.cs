using RomanticWeb.Entities;
using RomanticWeb.Model;
using VDS.RDF;

namespace RomanticWeb.Tests.Helpers
{
    public static class TripleExtentions
    {
        public static EntityQuad ToEntityQuad(this VDS.RDF.Triple triple, EntityId id)
        {
            return new EntityQuad(
                id,
                triple.Subject.ToNode(id),
                triple.Predicate.ToNode(id),
                triple.Object.ToNode(id),
                Node.ForUri(triple.GraphUri));
        }

        private static Node ToNode(this INode node, EntityId id)
        {
            if (node is IUriNode)
            {
                return Node.ForUri(((IUriNode)node).Uri);
            }

            if (node is IBlankNode)
            {
                return Node.ForBlank(((IBlankNode)node).InternalID, id, node.GraphUri);
            }

            var literal = (ILiteralNode)node;

            if (literal.DataType != null)
            {
                return Node.ForLiteral(literal.Value, literal.DataType);
            }

            if (literal.Language != null)
            {
                return Node.ForLiteral(literal.Value, literal.Language);
            }

            return Node.ForLiteral(literal.Value);
        }
    }
}