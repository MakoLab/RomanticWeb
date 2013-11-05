using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

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

		protected PropertyMap Collection<TReturnType>(Expression<Func<TEntity, TReturnType>> prop)
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
        private readonly ClassMap _class;

        protected EntityMap(Type type)
		{
			EntityType = type;
            _class=new ClassMap();
			MappedProperties = new List<PropertyMap>();
		}

		internal Type EntityType { get; set; }

		internal IList<PropertyMap> MappedProperties { get; private set; }

        protected ClassMap Class
        {
            get
            {
                return _class;
            }
        }

        internal EntityMapping CreateMapping(IOntologyProvider prefixes)
		{
			var entityMapping = new EntityMapping
			                        {
			                            Class=Class.GetMapping(prefixes)
			                        };

            foreach (var mappedProperty in MappedProperties)
			{
				entityMapping.Properties.Add(mappedProperty.GetMapping(prefixes));
			}

			return entityMapping;
		}
	}
}
