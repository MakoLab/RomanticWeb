using System;
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

        public object Convert(Node objectNode, IEntityContext context)
        {
            if (objectNode.IsLiteral)
            {
                return ConvertLiteral(objectNode,context);
                }

            return ConvertUri(objectNode,context);
        }

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

        private static bool ShouldConvertNodeToLiteral(Node objectNode, out Type type)
        {
            type = null;
            bool shouldConvert = false;

            // convert literal node
            shouldConvert |= objectNode.IsLiteral;
            
            if ((!shouldConvert))
            {
                // or convert primitive/string values
                shouldConvert |= type.IsPrimitive || type == typeof(string);
            }

            return shouldConvert;
        }

        private Node ConvertOneBack(object element)
        {
            if (element is IEntity)
            {
                return Node.FromEntityId(((IEntity)element).Id);
            }

            // todo: this is a hack, and should be in a complex type converter
            if (element is Uri)
            {
                return Node.ForUri((Uri)element);
            }

            ////var converter=_converters.LiteralNodeConverters.Where

            return Node.ForLiteral(element.ToString());
        }

        private object ConvertLiteral(Node objectNode,IEntityContext context)
        {
                var converter = Converters.GetBestConverter(objectNode);
                if (converter != null)
            {
                    return converter.Convert(objectNode, context);
            }

            throw new InvalidOperationException();
        }

        private object ConvertUri(Node uriNode,IEntityContext context)
        {
            return context.Load<IEntity>(uriNode.ToEntityId(),false);
        }
    }
}