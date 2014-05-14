using System;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>Estension methods for <see cref="INode"/>s.</summary>
    public static class NodeExtensions
    {
        /// <summary>Converts a dotNetRDF's <see cref="INode"/> into RomanticWeb's <see cref="Node"/>.</summary>
        public static Node WrapNode(this INode node,[AllowNull] EntityId entityId)
        {
            var literal=node as ILiteralNode;
            if (literal!=null)
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

            var uriNode=node as IUriNode;
            if (uriNode!=null)
            {
                return Node.ForUri(uriNode.Uri);
            }

            var blankNode=node as IBlankNode;
            if (blankNode!=null)
            {
                return Node.ForBlank(blankNode.InternalID,entityId,blankNode.GraphUri);
            }

            throw new ArgumentException("The node was neither URI, literal nor blank","node");
        }

        /// <summary>Converts a RomanticWeb's <see cref="Node"/> into dotNetRDF's <see cref="INode"/>.</summary>
        public static INode UnWrapNode(this Node node,INodeFactory nodeFactory)
        {
            if (node.IsUri)
            {
                return nodeFactory.CreateUriNode(node.Uri);
            }

            if (node.IsLiteral)
            {
                if (node.Language!=null)
                {
                    return nodeFactory.CreateLiteralNode(node.Literal,node.Language);
                }

                if (node.DataType!=null)
                {
                    return nodeFactory.CreateLiteralNode(node.Literal,node.DataType);
                }

                return nodeFactory.CreateLiteralNode(node.Literal);
            }

            return nodeFactory.CreateBlankNode(node.BlankNode);
        }

        /// <summary>Gets the graph node's Uri.</summary>
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

            return graphUriNode.Uri;
        }
    }
}