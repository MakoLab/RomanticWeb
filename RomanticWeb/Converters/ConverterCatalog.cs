using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    public sealed class ConverterCatalog : IConverterCatalog
    {
        internal ConverterCatalog(IEnumerable<INodeConverter> nodeConverters)
        {
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters { get; private set; }
    }
}