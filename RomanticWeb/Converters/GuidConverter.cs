using System;
using System.ComponentModel.Composition;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class GuidConverter:ILiteralNodeConverter
    {
        public object Convert(Node objectNode)
        {
            return Guid.Parse(objectNode.Literal);
        }

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