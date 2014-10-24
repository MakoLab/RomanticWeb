using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>Mapping provider, which returns a mapping for collection property predicate.</summary>
    public class CollectionMappingProvider : ICollectionMappingProvider
    {
        private readonly IPropertyMappingProvider _propertyMapping;
        private Type _elementConverterType;

        /// <summary>Initializes a new instance of the <see cref="CollectionMappingProvider" /> class.</summary>
        /// <param name="propertyMapping">The property mapping.</param>
        /// <param name="storeAs">The storage strategy.</param>
        public CollectionMappingProvider(IPropertyMappingProvider propertyMapping, StoreAs storeAs)
        {
            _propertyMapping = propertyMapping;
            ((ICollectionMappingProvider)this).StoreAs = storeAs;
        }

        /// <inheritdoc />
        public Func<IOntologyProvider, Uri> GetTerm
        {
            get { return _propertyMapping.GetTerm; }
            set { _propertyMapping.GetTerm = value; }
        }

        /// <inheritdoc />
        public PropertyInfo PropertyInfo { get { return _propertyMapping.PropertyInfo; } }

        /// <inheritdoc />
        public Type ConverterType
        {
            [return: AllowNull]
            get { return _propertyMapping.ConverterType; }
            set { _propertyMapping.ConverterType = value; }
        }

        /// <summary>Gets or sets the storage strategy.</summary>
        /// <remarks>Setting this updated the <see cref="Aggregation"/> property.</remarks>
        StoreAs ICollectionMappingProvider.StoreAs { get; set; }

        /// <inheritdoc/>
        public Type ElementConverterType
        {
            [return: AllowNull]
            get { return _elementConverterType; }
            set { _elementConverterType = value; }
        }

        /// <inheritdoc/>
        public void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _propertyMapping.ToString();
        }
    }
}