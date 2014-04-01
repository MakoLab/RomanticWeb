using System;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Defines a contract for converting literal nodes.</summary>
    public abstract class LiteralNodeConverter:INodeConverter
    {
        /// <summary>Check if a converter can convert the given RDF data type.</summary>
        public abstract LiteralConversionMatch CanConvert(Node literalNode);

        public object Convert(Node objectNode,IEntityContext context)
        {
            if (!objectNode.IsLiteral)
            {
                throw new ArgumentOutOfRangeException("objectNode","Node is not literal");
            }

            return ConvertInternal(objectNode);
        }

        public abstract Node ConvertBack(object value);

        protected abstract object ConvertInternal(Node literalNode);
    }
}