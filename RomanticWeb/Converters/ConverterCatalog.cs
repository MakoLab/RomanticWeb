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

        private readonly Func<IEnumerable<INodeConverter>> _getConverters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterCatalog"/> class.
        /// </summary>
        public ConverterCatalog(Func<Type, INodeConverter> createConverter, Func<IEnumerable<INodeConverter>> getConverters)
        {
            _createConverter = createConverter;
            _getConverters = getConverters;
        }

        internal ConverterCatalog()
            : this(type => null, () => new INodeConverter[0])
        {
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<INodeConverter>(_getConverters().ToList());
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<ILiteralNodeConverter>(_getConverters().Where(c => c is LiteralNodeConverter).Cast<ILiteralNodeConverter>().ToList());
            }
        }

        /// <inheritdoc/>
        public INodeConverter GetConverter(Type converterType)
        {
            if (converterType == typeof(FallbackNodeConverter))
            {
                return new FallbackNodeConverter(this);
            }

            try
            {
                return _createConverter(converterType);
            }
            catch
            {
                return (INodeConverter)Activator.CreateInstance(converterType);
            }
        }
    }
}