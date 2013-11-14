using System;
using System.Reflection;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for properties
    /// </summary>
    public class PropertyMap:TermMap,INamedGraphSelectingMap
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
        /// Gets a named graph mapping part
        /// </summary>
        public NamedGraphPart<PropertyMap> NamedGraph
        {
            get { return new NamedGraphPart<PropertyMap>(this); }
        }

        /// <inheritdoc />
        IGraphSelectionStrategy INamedGraphSelectingMap.GraphSelector { get; set; }

        /// <summary>
        /// Gets a value indicating whether this property is a collection
        /// </summary>
        protected internal virtual bool IsCollection { get { return false; } }

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
            return new PropertyMapping(
                PropertyInfo.PropertyType,
                PropertyInfo.Name,
                GetTermUri(mappingContext.OntologyProvider),
                ((INamedGraphSelectingMap)this).GraphSelector ?? mappingContext.DefaultGraphSelector);
        }
    }
}