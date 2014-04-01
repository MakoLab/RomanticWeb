using System.Reflection;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <inheritdoc/>
    public sealed class CollectionMap:PropertyMapBase,ICollectionMap
    {
        internal CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

        public ITermPart<ICollectionMap> Term
        {
            get
            {
                return new TermPart<CollectionMap>(this);
            }
        }

        /// <summary>
        /// Gets options for setting how this collection will be persisted
        /// </summary>
        public StorageStrategyPart StoreAs
        {
            get
            {
                return new StorageStrategyPart(this);
            }
        }

        /// <summary>
        /// Gets or sets the value, which indicates how the collection's triples will be stored.
        /// </summary>
        public StoreAs StorageStrategy { get; set; }

        /// <inheritdoc />
        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }

        public ICollectionMap ConvertElementsWith<TConverter>()
            where TConverter:INodeConverter
        {
            ConverterType=typeof(TConverter);
            return this;
        }
    }
}