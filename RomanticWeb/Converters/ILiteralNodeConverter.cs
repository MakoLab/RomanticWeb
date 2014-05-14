using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Contract for implementing literal node converters
    /// </summary>
    public interface ILiteralNodeConverter:INodeConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified literal node.
        /// </summary>
        /// <param name="literalNode">The literal node.</param>
        LiteralConversionMatch CanConvert(Node literalNode);
    }
}