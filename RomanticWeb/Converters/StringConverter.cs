using System;
using NullGuard;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>Converts string literals.</summary>
    public class StringConverter : LiteralNodeConverter
    {
        /// <inheritdoc/>
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(value.ToString(), Xsd.String);
        }

        /// <inheritdoc/>
        public override LiteralConversionMatch CanConvert(Node literalNode)
        {
            var literalConversionMatch = new LiteralConversionMatch { LiteralFormatMatches = MatchResult.DontCare };
            if ((literalNode.IsLiteral) && ((literalNode.DataType == null) || (AbsoluteUriComparer.Default.Compare(literalNode.DataType, Xsd.String) == 0)))
            {
                literalConversionMatch.DatatypeMatches = MatchResult.ExactMatch;
            }

            return literalConversionMatch;
        }

        /// <inheritdoc/>
        [return: AllowNull]
        public override Uri CanConvertBack(Type type)
        {
            return (type == typeof(string) ? Xsd.String : null);
        }

        /// <inheritdoc/>
        protected override object ConvertInternal(Node objectNode)
        {
            if (!objectNode.IsLiteral)
            {
                return objectNode.ToString();
            }

            return objectNode.Literal;
        }
    }
}