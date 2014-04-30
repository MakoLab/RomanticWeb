﻿using System;
using System.Collections;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Default converter for <see cref="Node"/>s to value objects or entities.</summary>
    public sealed class FallbackNodeConverter : INodeConverter
    {
        private static readonly IConverterCatalog Converters;

        /// <summary>Constructor with entity context passed.</summary>
        static FallbackNodeConverter()
        {
            Converters=new ConverterCatalog();
        }

        /// <summary>
        /// Converts a node to a correct value based on type (URI, blank or literal)
        /// or it's datatype in the case of literal nodes
        /// </summary>
        public object Convert(Node objectNode, IEntityContext context)
        {
            if (objectNode.IsLiteral)
            {
                return ConvertLiteral(objectNode,context);
            }

            return ConvertUri(objectNode,context);
        }

        /// <inheritdoc/>
        public Node ConvertBack(object value)
        {
            if (value is IEntity)
            {
                return Node.FromEntityId(((IEntity)value).Id);
            }
            
            if (value is IEnumerable&&!(value is string))
            {
                throw new InvalidOperationException();
            }
            
            return ConvertOneBack(value);
        }

        private static Node ConvertOneBack(object element)
        {
            if (element is IEntity)
            {
                return Node.FromEntityId(((IEntity)element).Id);
            }

            if (element is Uri)
            {
                return Node.ForUri((Uri)element);
            }
            
            return Node.ForLiteral(element.ToString());
        }

        private static object ConvertLiteral(Node objectNode,IEntityContext context)
        {
            var converter=Converters.GetBestConverter(objectNode);
            if (converter!=null)
            {
                return converter.Convert(objectNode,context);
            }

            throw new InvalidOperationException();
        }

        private static object ConvertUri(Node uriNode,IEntityContext context)
        {
            return context.Load<IEntity>(uriNode.ToEntityId(),false);
        }
    }
}