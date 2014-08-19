using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    public sealed class ConverterCatalog : IConverterCatalog
    {
        private readonly IEnumerable<INodeConverter> _nodeConverters;

        internal ConverterCatalog(IEnumerable<INodeConverter> nodeConverters)
        {
            _nodeConverters = nodeConverters;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<INodeConverter>(_nodeConverters.ToList());
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<ILiteralNodeConverter>(_nodeConverters.Where(c => c is LiteralNodeConverter).Cast<ILiteralNodeConverter>().ToList());
            }
        }
    }
}