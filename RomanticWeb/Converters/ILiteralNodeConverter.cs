using System;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Contract for implementing literal node converters.</summary>
    public interface ILiteralNodeConverter : INodeConverter
    {
        /// <summary>Determines whether this instance can convert the specified literal node.</summary>
        /// <param name="literalNode">The literal node.</param>
        LiteralConversionMatch CanConvert(Node literalNode);

        /// <summary>Determines whether this instance can convert given strong type to literal.</summary>
        /// <param name="type">Type to be converted.</param>
        /// <returns><see cref="Uri" /> if this converted can convert given type, otherwise <b>null</b>.</returns>
        Uri CanConvertBack(Type type);
    }
}