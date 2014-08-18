using System;
using System.Collections;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Default converter for <see cref="Node"/>s to value objects or entities.</summary>
    public sealed class FallbackNodeConverter : INodeConverter
    {
        private readonly IConverterCatalog _converters;

        /// <summary>Constructor with entity context passed.</summary>
        public FallbackNodeConverter(IEnumerable<INodeConverter> converters)
        {
            this._converters = new ConverterCatalog(converters);
        }

        public FallbackNodeConverter()
            : this(new INodeConverter[0])
        {
        }

        /// <summary>
        /// Converts a node to a correct value based on type (URI, blank or literal)
        /// or it's datatype in the case of literal nodes
        /// </summary>
        public object Convert(Node objectNode, IEntityContext context)
        {
            if (objectNode.IsLiteral)
            {
                return ConvertLiteral(objectNode, context);
            }

            return ConvertUri(objectNode, context);
        }

        /// <inheritdoc/>
        public Node ConvertBack(object value)
        {
            if (value is IEntity)
            {
                return Node.FromEntityId(((IEntity)value).Id);
            }

            if (value is IEnumerable && !(value is string))
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

        private object ConvertLiteral(Node objectNode, IEntityContext context)
        {
            var converter = _converters.GetBestConverter(objectNode);
            if (converter != null)
            {
                return converter.Convert(objectNode, context);
            }

            throw new InvalidOperationException();
        }

        private object ConvertUri(Node uriNode, IEntityContext context)
        {
            return context.Load<IEntity>(uriNode.ToEntityId());
        }
    }
}