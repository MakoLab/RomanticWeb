using System;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a collection to an RDF predicate.</summary>
    public class CollectionAttribute:PropertyAttribute
    {
        private StorageStrategyOption _storageStrategy;

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
        /// Initializes a new instance of the <see cref="CollectionAttribute"/> class.
        /// </summary>
        /// <param name="propertyUri">The property URI.</param>
        public CollectionAttribute(string propertyUri)
            :base(propertyUri)
        {
        }

        /// <summary>
        /// Gets or sets the storage strategy
        /// </summary>
        public StorageStrategyOption StorageStrategy
        {
            get
            {
                return _storageStrategy;
            }

            set
            {
                switch (value)
                {
                    case StorageStrategyOption.Simple:
                        Aggregation=Entities.ResultAggregations.Aggregation.Original;
                        break;
                    case StorageStrategyOption.RdfList:
                        Aggregation=Entities.ResultAggregations.Aggregation.SingleOrDefault;
                        break;
                    default:
                        Aggregation=null;
                        break;
                }

                _storageStrategy=value;
            }
        }

        private Aggregation? Aggregation { get; set; }

        /// <summary>
        /// Creates a <see cref="CollectionMapping"/>
        /// </summary>
        protected override IPropertyMapping GetMappingInternal(Type propertyType, string propertyName, Uri uri, MappingContext mappingContext)
        {
            var collectionMapping = new CollectionMapping(propertyType, propertyName, uri, StorageStrategy);

            if (StorageStrategy!=StorageStrategyOption.None)
            {
                collectionMapping.StorageStrategy=StorageStrategy;
            }

            collectionMapping.Aggregation = Aggregation;

            return collectionMapping;
        }
    }
}