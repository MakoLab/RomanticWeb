using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    internal sealed class ConverterCatalog : IConverterCatalog
    {
        private readonly Func<Type, INodeConverter> _createConverter;

        private readonly IDictionary<Type, INodeConverter> _converters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterCatalog"/> class.
        /// </summary>
        public ConverterCatalog(Func<Type, INodeConverter> createConverter)
        {
            _createConverter = createConverter;
        }

        internal ConverterCatalog()
            : this(type => null)
        {
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<INodeConverter>(_converters.Values.ToList());
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<ILiteralNodeConverter>(_converters.Values.Where(c => c is LiteralNodeConverter).Cast<ILiteralNodeConverter>().ToList());
            }
        }

        /// <inheritdoc/>
        public INodeConverter GetConverter(Type converterType)
        {
            if (converterType == typeof(FallbackNodeConverter))
            {
                return new FallbackNodeConverter(this);
            }

            return _createConverter(converterType);
        }
    }
}