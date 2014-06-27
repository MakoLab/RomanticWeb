using NullGuard;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>Converts Base64 literals.</summary>
    public class Base64BinaryConverter:LiteralNodeConverter
    {
        /// <summary>Converts given array of bytes to it's base64 representation.</summary>
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(System.Convert.ToBase64String((byte[])value),Xsd.Base64Binary);
        }

        /// <summary>Checks for ability to convert given data type.</summary>
        /// <param name="literalNode"></param>
        /// <returns><b>true</b> if the data type is Base64 binary; otherwise <b>false</b>.</returns>
        public override LiteralConversionMatch CanConvert(Node literalNode)
        {
            var match=new LiteralConversionMatch { LiteralFormatMatches=MatchResult.DontCare };
            if (new AbsoluteUriComparer().Equals(literalNode.DataType,Xsd.Base64Binary))
            {
                match.DatatypeMatches=MatchResult.ExactMatch;
            }

            return match;
        }

        /// <inheritdoc />
        [return: AllowNull]
        public override System.Uri CanConvertBack(System.Type type)
        {
            return (type==typeof(byte[])?Xsd.Base64Binary:null);
        }

        /// <summary>Converts given Base64 binary literal into an array of bytes.</summary>
        /// <param name="objectNode">Node with Base64 binary literal.</param>
        /// <returns>Array of bytes or null if the passed node is also null.</returns>
        protected override object ConvertInternal(Node objectNode)
        {
            return (objectNode.Literal!=null?System.Convert.FromBase64String(objectNode.Literal):null);
        }
    }
}