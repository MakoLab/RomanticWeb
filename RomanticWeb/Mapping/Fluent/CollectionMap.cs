using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <inheritdoc/>
    public sealed class CollectionMap : PropertyMapBase, ICollectionMap
    {
        private ITermPart<ICollectionMap> _term;
        private StorageStrategyPart _storeAs;
        private Type _elementConverterType = null;

        internal CollectionMap(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            _term = new TermPart<CollectionMap>(this);
            _storeAs = new StorageStrategyPart(this);
        }

        /// <inheritdoc/>
        public ITermPart<ICollectionMap> Term { get { return _term; } }

        /// <summary>Gets options for setting how this collection will be persisted.</summary>
        public StorageStrategyPart StoreAs { get { return _storeAs; } }

        /// <inheritdoc/>
        public override Type ConverterType
        {
            [return: AllowNull]
            get { return (StorageStrategy == Model.StoreAs.SimpleCollection ? _elementConverterType ?? base.ConverterType : base.ConverterType); }
            internal set { base.ConverterType = value; }
        }

        /// <summary>Gets or sets an element converter type.</summary>
        public Type ElementConverterType
        {
            [return: AllowNull]
            get { return (StorageStrategy == Model.StoreAs.SimpleCollection ? _elementConverterType ?? base.ConverterType : _elementConverterType); }
            internal set { _elementConverterType = value; }
        }

        /// <summary>Gets or sets the value, which indicates how the collection's triples will be stored.</summary>
        public StoreAs StorageStrategy { get; set; }

        /// <inheritdoc />
        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }

        /// <inheritdoc />
        public ICollectionMap ConvertElementsWith<TConverter>() where TConverter : INodeConverter
        {
            _elementConverterType = typeof(TConverter);
            return this;
        }
    }
}