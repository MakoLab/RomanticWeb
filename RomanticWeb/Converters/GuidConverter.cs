using System;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converter for GUID literal nodes
    /// </summary>
    public class GuidConverter:ILiteralNodeConverter
    {
        /// <inheritdoc />
        public object Convert(Node objectNode)
        {
            return Guid.Parse(objectNode.Literal);
        }

        /// <inheritdoc />
        public LiteralConversionMatch CanConvert(Node literalNode)
        {
            var result=new LiteralConversionMatch { DatatypeMatches=MatchResult.DontCare };

            Guid value;
            if (Guid.TryParse(literalNode.Literal,out value))
            {
                result.LiteralFormatMatches=MatchResult.ExactMatch;
            }

            return result;
        }
    }
}