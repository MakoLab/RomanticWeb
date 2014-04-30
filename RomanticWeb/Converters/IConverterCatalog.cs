using System.Collections.Generic;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Contract for implementing a catalog converter
    /// </summary>
    public interface IConverterCatalog
    {
        /// <summary>
        /// Gets the URI node converters.
        /// </summary>
        /// <value>
        /// The URI node converters.
        /// </value>
        IReadOnlyCollection<INodeConverter> UriNodeConverters { get; }

        /// <summary>
        /// Gets the literal node converters.
        /// </summary>
        /// <value>
        /// The literal node converters.
        /// </value>
        IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters { get; }
    }
}