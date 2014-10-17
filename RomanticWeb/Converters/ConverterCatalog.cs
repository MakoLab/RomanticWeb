using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RomanticWeb.LightInject;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    public sealed class ConverterCatalog : IConverterCatalog
    {
        private readonly IDictionary<Type, INodeConverter> _nodeConverters = new Dictionary<Type, INodeConverter>();
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterCatalog"/> class.
        /// </summary>
        public ConverterCatalog(IEnumerable<INodeConverter> converters)
            : this(new ServiceContainer())
        {
            foreach (var converter in converters)
            {
                AddConverter(converter);
            }
        }

        internal ConverterCatalog()
            : this(new INodeConverter[0])
        {
        }

        private ConverterCatalog(IServiceContainer container)
        {
            _container = container;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<INodeConverter> UriNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<INodeConverter>(_nodeConverters.Values.ToList());
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ILiteralNodeConverter> LiteralNodeConverters
        {
            get
            {
                return new ReadOnlyCollection<ILiteralNodeConverter>(_nodeConverters.Values.Where(c => c is LiteralNodeConverter).Cast<ILiteralNodeConverter>().ToList());
            }
        }

        /// <inheritdoc/>
        public INodeConverter GetConverter(Type converterType)
        {
            if (!_nodeConverters.ContainsKey(converterType))
            {
                AddConverter(CreateConverter(converterType));
            }

            return _nodeConverters[converterType];
        }

        /// <inheritdoc/>
        public void AddConverter(INodeConverter nodeConverter)
        {
            _nodeConverters[nodeConverter.GetType()] = nodeConverter;
        }

        private INodeConverter CreateConverter(Type converterType)
        {
            if (converterType == typeof(FallbackNodeConverter))
            {
                return new FallbackNodeConverter(this);
            }

            if (!_container.CanGetInstance(converterType, System.String.Empty))
            {
                _container.RegisterInstance(converterType, (INodeConverter)Activator.CreateInstance(converterType));
            }

            return (INodeConverter)_container.GetInstance(converterType);
        }
    }
}