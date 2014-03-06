using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for properties
    /// </summary>
    public class PropertyMap:TermMap
    {
		private readonly PropertyInfo _propertyInfo;

        internal PropertyMap(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
		public TermPart<PropertyMap> Term
		{
			get
			{
				return new TermPart<PropertyMap>(this);
			}
		}

        /// <summary>
        /// Not used for property map
        /// </summary>
        /// <returns><see cref="StorageStrategyOption.None"/></returns>
        /// <remarks>Setting throws <see cref="InvalidOperationException"/></remarks>
        protected internal virtual StorageStrategyOption StorageStrategy
        {
            get
            {
                return StorageStrategyOption.None;
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        protected internal virtual Aggregation? Aggregation
        {
            get
            {
                return null;
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the mapped property's <see cref="System.Reflection.PropertyInfo"/>
        /// </summary>
        protected PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

        /// <summary>
        /// Creates a mapping from this definition
        /// </summary>
        protected internal virtual IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            var propertyMapping=new PropertyMapping(PropertyInfo.PropertyType,PropertyInfo.Name,GetTermUri(mappingContext.OntologyProvider));

            if (StorageStrategy!=StorageStrategyOption.None)
            {
                propertyMapping.StorageStrategy=StorageStrategy;
            }
            
            return propertyMapping;
        }
    }
}