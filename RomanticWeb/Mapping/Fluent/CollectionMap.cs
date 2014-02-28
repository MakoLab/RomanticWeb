using System.Reflection;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for collection properties
    /// </summary>
	public class CollectionMap : PropertyMap
    {
        internal CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        public new TermPart<CollectionMap> Term
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

        /// <inheritdoc />
        /// <returns>true</returns>
        protected internal override bool IsCollection
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected internal override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            return new CollectionMapping(
                PropertyInfo.PropertyType,
                PropertyInfo.Name,
                GetTermUri(mappingContext.OntologyProvider),
                StorageStrategy);
        }
	}
}