using System;
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
        IEnumerable<INodeConverter> UriNodeConverters { get; }

        /// <summary>
        /// Gets the literal node converters.
        /// </summary>
        IEnumerable<ILiteralNodeConverter> LiteralNodeConverters { get; }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <param name="converterType">Type of the converter.</param>
        INodeConverter GetConverter(Type converterType);

        /// <summary>
        /// Adds a converter.
        /// </summary>
        /// <param name="nodeConverter">The node converter.</param>
        void AddConverter(INodeConverter nodeConverter);
    }
}