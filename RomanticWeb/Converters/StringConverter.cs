using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    public class StringConverter:LiteralNodeConverter
    {
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(value.ToString(),Xsd.String);
        }

        public override LiteralConversionMatch CanConvert(Node literalNode)
        {
            var literalConversionMatch=new LiteralConversionMatch
                                           {
                                               LiteralFormatMatches=MatchResult.DontCare
                                           };

            if (literalNode.IsLiteral&&(literalNode.DataType==null||new AbsoluteUriComparer().Compare(literalNode.DataType,Xsd.String)==0))
            {
                literalConversionMatch.DatatypeMatches=MatchResult.ExactMatch;
            }

            return literalConversionMatch;
        }

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