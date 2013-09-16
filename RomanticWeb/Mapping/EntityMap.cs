using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RomanticWeb.Mapping
{
    public abstract class EntityMap<TEntity> : EntityMap
    {
        protected EntityMap()
            : base(typeof(TEntity))
        {

        }

        protected PropertyMap Property<TReturnType>(Expression<Func<TEntity, TReturnType>> prop)
        {
            var propertyMap = new PropertyMap(prop.ExtractPropertyInfo());

            MappedProperties.Add(propertyMap);

            return propertyMap;
        }

        protected PropertyMap Collection<TReturnType>(Expression<Func<TEntity, TReturnType>> prop)
        {
            var propertyMap = new CollectionMap(prop.ExtractPropertyInfo());

            MappedProperties.Add(propertyMap);

            return propertyMap;
        }
    }

    public abstract class EntityMap : IMappingProvider
    {
        protected EntityMap(Type type)
        {
            EntityType = type;
            MappedProperties = new List<PropertyMap>();
        }

        internal Type EntityType { get; set; }
        internal IList<PropertyMap> MappedProperties { get; private set; }

        IMapping IMappingProvider.GetMapping()
        {
            var entityMapping = new EntityMapping();

            foreach (var mappedProperty in MappedProperties)
            {
                entityMapping.Properties.Add(mappedProperty.GetMapping());
            }

            return entityMapping;
        }
    }
}
