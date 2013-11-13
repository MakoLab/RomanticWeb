using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Base class for fluently defining entity entityMappings
    /// </summary>
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

        protected CollectionMap Collection<TReturnType>(Expression<Func<TEntity, TReturnType>> prop)
		{
			var propertyMap = new CollectionMap(prop.ExtractPropertyInfo());

			MappedProperties.Add(propertyMap);

			return propertyMap;
		}
	}

    /// <summary>
    /// Base class for fluently defining entity entityMappings
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic and non-generic flavour of same class")]
    public abstract class EntityMap
    {
        private ClassMap _class;

        protected EntityMap(Type type)
		{
			EntityType = type;
			MappedProperties = new List<PropertyMap>();
		}

		internal Type EntityType { get; set; }

		internal IList<PropertyMap> MappedProperties { get; private set; }

        protected ClassMap Class
        {
            get
            {
                return _class??(_class=new ClassMap());
            }
        }

        internal EntityMapping CreateMapping(MappingContext mappingContext)
		{
			var entityMapping = new EntityMapping();

            if (_class!=null)
            {
                entityMapping.Class=_class.GetMapping(mappingContext);
            }

            foreach (var mappedProperty in MappedProperties)
			{
				entityMapping.Properties.Add(mappedProperty.GetMapping(mappingContext));
			}

			return entityMapping;
		}
	}
}
