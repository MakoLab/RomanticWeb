using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for collection properties
    /// </summary>
	public class CollectionMap : PropertyMapBase<CollectionMap>
    {
        internal CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        public override ITermPart<CollectionMap> Term
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
        /// Gets the storage strategy of this collection
        /// </summary>
        protected internal override StorageStrategyOption StorageStrategy { get; set; }

        protected internal override Aggregation? Aggregation { get; set; }

        /// <inheritdoc />
        public override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            var collectionMapping=new CollectionMapping(PropertyInfo.PropertyType,PropertyInfo.Name,GetTermUri(mappingContext.OntologyProvider),StorageStrategy);

            if (StorageStrategy != StorageStrategyOption.None)
            {
                collectionMapping.StorageStrategy = StorageStrategy;
            }

            collectionMapping.Aggregation=Aggregation;

            return collectionMapping;
        }
    }
}