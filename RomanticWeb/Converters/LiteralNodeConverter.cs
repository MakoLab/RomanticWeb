using System;
using NullGuard;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Defines a contract for converting literal nodes.</summary>
    public abstract class LiteralNodeConverter : ILiteralNodeConverter
    {
        /// <summary>Check if a converter can convert the given RDF data type.</summary>
        public abstract LiteralConversionMatch CanConvert(Node literalNode);

        /// <summary>Converts the given node to an object.</summary>
        public object Convert(Node objectNode, IEntityContext context)
        {
            if (!objectNode.IsLiteral)
            {
                throw new ArgumentOutOfRangeException("objectNode", "Node is not literal");
            }

            return ConvertInternal(objectNode);
        }

        /// <summary>Converts the given value to a literal node.</summary>
        [return: AllowNull]
        public abstract Node ConvertBack(object value, IEntityContext context);

        /// <inheritdoc />
        public abstract Uri CanConvertBack(Type type);

        /// <summary>Does the actual convertion.</summary>
        protected abstract object ConvertInternal(Node literalNode);
    }
}