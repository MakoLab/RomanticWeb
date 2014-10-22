using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for dictionary property predicate
    /// </summary>
    public class DictionaryMappingProvider : IDictionaryMappingProvider
    {
        private readonly IPropertyMappingProvider _property;

        private readonly ITermMappingProvider _key;
        private readonly ITermMappingProvider _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMappingProvider"/> class.
        /// </summary>
        /// <param name="key">The key mapping provider.</param>
        /// <param name="value">The value mapping provider.</param>
        /// <param name="property">The property.</param>
        public DictionaryMappingProvider(IPropertyMappingProvider property, ITermMappingProvider key, ITermMappingProvider value)
        {
            _property = property;
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Gets the key mapping provider.
        /// </summary>
        /// <value>
        /// The key mapping provider.
        /// </value>
        public ITermMappingProvider Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Gets the value mapping provider.
        /// </summary>
        /// <value>
        /// The value mapping provider.
        /// </value>
        public ITermMappingProvider Value
        {
            get
            {
                return _value;
            }
        }

        /// <inheritdoc/>
        public Func<IOntologyProvider, Uri> GetTerm
        {
            get
            {
                return _property.GetTerm;
            }

            set
            {
                _property.GetTerm = value;
            }
        }

        /// <inheritdoc/>
        public PropertyInfo PropertyInfo
        {
            get
            {
                return _property.PropertyInfo;
            }
        }

        /// <inheritdoc/>
        public Type ConverterType
        {
            [return: AllowNull]
            get
            {
                return _property.ConverterType;
            }

            set
            {
                _property.ConverterType = value;
            }
        }

        /// <inheridoc/>
        public void Accept(Visitors.IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _property.ToString();
        }
    }
}