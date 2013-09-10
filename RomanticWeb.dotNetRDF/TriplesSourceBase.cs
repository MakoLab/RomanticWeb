using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public abstract class TriplesSourceBase : ITriplesSource
    {
        protected static RdfNode WrapObjectNode(INode arg)
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

        public abstract IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate);
    }
}