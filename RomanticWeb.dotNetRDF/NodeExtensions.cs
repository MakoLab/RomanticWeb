using System;
using RomanticWeb.Model;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
	internal static class NodeExtensions
	{
		public static Node WrapNode(this INode node)
		{
			var literal = node as ILiteralNode;
			if (literal != null)
			{
                if (literal.DataType!=null)
                {
                    return Node.ForLiteral(literal.Value,literal.DataType);
                }
                
                if (literal.Language!=null)
                {
                    return Node.ForLiteral(literal.Value,literal.Language);
                }

				return Node.ForLiteral(literal.Value);
			}

			var uriNode = node as IUriNode;
			if (uriNode != null)
			{
				return Node.ForUri(uriNode.Uri);
			}

			var blankNode = node as IBlankNode;
			if (blankNode != null)
			{
				return Node.ForBlank(blankNode.InternalID, blankNode.GraphUri);
			}

			throw new ArgumentException("The node was neither URI, literal nor blank", "node");
		}
	}
}