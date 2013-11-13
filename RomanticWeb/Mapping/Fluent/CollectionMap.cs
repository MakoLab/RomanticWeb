using System;
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
        public new PredicatePart<CollectionMap> Predicate
        {
            get
            {
                return new PredicatePart<CollectionMap>(this);
            }
        }

        /// <summary>
        /// Gets a named graph mapping part
        /// </summary>
        public new NamedGraphPart<CollectionMap> NamedGraph
        {
            get { return new NamedGraphPart<CollectionMap>(this); }
        }

        public StorageStrategyPart StoreAs
        {
            get
            {
                return new StorageStrategyPart(this);
            }
        }

        internal StorageStrategyOption StorageStrategy { get; set; }

        /// <summary>
        /// Returns true
        /// </summary>
        protected internal override bool IsCollection
        {
            get { return true; }
        }

        protected internal override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            Uri predicateUri = PredicateUri ?? mappingContext.OntologyProvider.ResolveUri(NamespacePrefix, PredicateName);
            return new CollectionMapping(
                PropertyInfo.PropertyType,
                PropertyInfo.Name,
                predicateUri,
                ((INamedGraphSelectingMap)this).GraphSelector??mappingContext.DefaultGraphSelector,
                StorageStrategy);
        }
	}
}