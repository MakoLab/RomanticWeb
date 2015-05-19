using System;
using System.Text.RegularExpressions;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Converter for GUID literal nodes.</summary>
    public class GuidConverter : INodeConverter
    {
        private static readonly Regex UrnUuidRegex = new Regex(@"^urn:uuid:", RegexOptions.IgnoreCase);

        /// <inheritdoc />
        public object Convert(Node objectNode, IEntityContext context)
        {
            if (objectNode.IsLiteral)
            {
                return Guid.Parse(objectNode.Literal);
            }

            if (objectNode.IsUri && UrnUuidRegex.IsMatch(objectNode.Uri.ToString()))
            {
                Guid guid;
                if (Guid.TryParse(UrnUuidRegex.Replace(objectNode.Uri.ToString(), string.Empty), out guid))
                {
                    return guid;
                }
            }

            throw new ArgumentException(string.Format("Cannot convert node '{0}' to guid", objectNode), "objectNode");
        }

        /// <inheritdoc />
        public Node ConvertBack(object value, IEntityContext context)
        {
            return Node.ForLiteral(value.ToString());
        }

        /// <inheritdoc />
        public LiteralConversionMatch CanConvert(Node literalNode)
        {
            var result = new LiteralConversionMatch { DatatypeMatches = MatchResult.DontCare };

            Guid value;
            if (Guid.TryParse(literalNode.Literal, out value))
            {
                result.LiteralFormatMatches = MatchResult.ExactMatch;
            }

            return result;
        }
    }
}