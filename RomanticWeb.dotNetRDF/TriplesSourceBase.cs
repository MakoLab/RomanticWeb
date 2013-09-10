using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public abstract class TriplesSourceBase : ITriplesSource
    {
        protected static RdfNode WrapObjectNode(INode node)
        {
            var literal = node as ILiteralNode;
            if (literal != null)
            {
                return RdfNode.ForLiteral(literal.Value, literal.Language, literal.DataType);
            }
            var uriNode = node as IUriNode;
            if (uriNode != null)
            {
                return RdfNode.ForUri(uriNode.Uri);
            }
            
            var blankNode = node as IBlankNode;
            if(blankNode != null)
            {
                return RdfNode.ForBlank(blankNode.InternalID, blankNode.GraphUri);
            }

            throw new ArgumentException("The node was neither URI, literal nor blank", "node");
        }

        public abstract IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate);
    }
}