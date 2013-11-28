using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a collection to an RDF predicate.</summary>
    public class CollectionAttribute:PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAttribute"/> class.
        /// </summary>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="propertyName">Predicate name.</param>
        public CollectionAttribute(string prefix,string propertyName)
            :base(prefix,propertyName)
        {
            StorageStrategy = StorageStrategyOption.Simple;
        }

        /// <summary>
        /// Gets or sets the storage strategy
        /// </summary>
        public StorageStrategyOption StorageStrategy { get; set; }

        /// <summary>
        /// Creates a <see cref="CollectionMapping"/>
        /// </summary>
        protected override IPropertyMapping GetMappingInternal(Type propertyType, string propertyName, Uri uri, MappingContext mappingContext)
        {
            return new CollectionMapping(propertyType, propertyName, uri, mappingContext.DefaultGraphSelector, StorageStrategy);
        }
    }
}