using System;
using RomanticWeb.Model;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
	internal static class NodeExtensions
	{
	    private static readonly INodeFactory NodeFactory=new NodeFactory();

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

        public static INode UnWrapNode(this Node node, INodeFactory nodeFactory)
        {
            if (node.IsUri)
            {
                return nodeFactory.CreateUriNode(node.Uri);
            }
            
            if (node.IsLiteral)
            {
                if (node.Language != null)
                {
                    return nodeFactory.CreateLiteralNode(node.Literal,node.Language);
                }

                if (node.DataType!=null)
                {
                    return nodeFactory.CreateLiteralNode(node.Literal,node.DataType);
                }

                return nodeFactory.CreateLiteralNode(node.Literal);
            }

            throw new NotImplementedException("Cannot unwrap blank nodes yet");
        }

        public static Uri UnWrapGraphUri(this Node graphUriNode)
        {
            if (graphUriNode==null)
            {
                return null;
            }

            if (!graphUriNode.IsUri)
            {
                throw new ArgumentException("Graphs can only be URI nodes");
            }

            return ((IUriNode)UnWrapNode(graphUriNode, NodeFactory)).Uri;
        }
	}
}