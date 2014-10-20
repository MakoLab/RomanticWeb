using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Default implementation of <see cref="IConverterCatalog"/>
    /// </summary>
    internal sealed class ConverterCatalog : IConverterCatalog
    {
        private readonly IDictionary<Type, INodeConverter> _nodeConverters = new Dictionary<Type, INodeConverter>();
        private readonly IServiceContainer _container = new ServiceContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterCatalog"/> class.
        /// </summary>
        public ConverterCatalog()
        {
            _container.RegisterAssembly(GetType().Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterCatalog"/> class.
        /// </summary>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy.</param>
        public ConverterCatalog(IBaseUriSelectionPolicy baseUriSelectionPolicy) : this()
        {
            _container.RegisterInstance<IBaseUriSelectionPolicy>(baseUriSelectionPolicy);
            _nodeConverters = _container.GetAllInstances<INodeConverter>().ToDictionary(item => item.GetType(), item => item);
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
                INodeConverter nodeConverter = CreateConverter(converterType);
                _nodeConverters[nodeConverter.GetType()] = nodeConverter;
            }

            return _nodeConverters[converterType];
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