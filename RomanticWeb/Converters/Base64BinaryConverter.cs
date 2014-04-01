using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>Converts Base64 literals.</summary>
    public class Base64BinaryConverter:INodeConverter
    {
        /// <summary>Converts given Base64 binary literal into an array of bytes.</summary>
        /// <param name="objectNode">Node with Base64 binary literal.</param>
        /// <param name="context"></param>
        /// <returns>Array of bytes or null if the passed node is also null.</returns>
        public object Convert(Node objectNode,IEntityContext context)
        {
            return (objectNode.Literal!=null?System.Convert.FromBase64String(objectNode.Literal):null);
        }

        /// <summary>Converts given array of bytes to it's base64 representation.</summary>
        public Node ConvertBack(object value)
        {
            return Node.ForLiteral(System.Convert.ToBase64String((byte[])value),Xsd.Base64Binary);
        }

        /// <summary>Checks for ability to convert given data type.</summary>
        /// <param name="literalNode"></param>
        /// <returns><b>true</b> if the data type is Base64 binary; otherwise <b>false</b>.</returns>
        public LiteralConversionMatch CanConvert(Node literalNode)
        {
            var match=new LiteralConversionMatch
                       {
                           LiteralFormatMatches = MatchResult.DontCare
                       };

            if (new AbsoluteUriComparer().Equals(literalNode.DataType,Xsd.Base64Binary))
            {
                match.DatatypeMatches=MatchResult.ExactMatch;
            }

            return match;
        }
    }
}