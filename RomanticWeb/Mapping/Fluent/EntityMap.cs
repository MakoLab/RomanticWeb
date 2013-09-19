using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
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

			this.MappedProperties.Add(propertyMap);

			return propertyMap;
		}

		protected PropertyMap Collection<TReturnType>(Expression<Func<TEntity, TReturnType>> prop)
		{
			var propertyMap = new CollectionMap(prop.ExtractPropertyInfo());

			this.MappedProperties.Add(propertyMap);

			return propertyMap;
		}
	}

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic and non-generic flavour of same class")]
    public abstract class EntityMap : IMappingProvider
	{
		protected EntityMap(Type type)
		{
			this.EntityType = type;
			this.MappedProperties = new List<PropertyMap>();
		}

		internal Type EntityType { get; set; }

		internal IList<PropertyMap> MappedProperties { get; private set; }

        IMapping IMappingProvider.GetMapping(IOntologyProvider prefixes)
		{
			var entityMapping = new EntityMapping();

			foreach (var mappedProperty in this.MappedProperties)
			{
				entityMapping.Properties.Add(mappedProperty.GetMapping(prefixes));
			}

			return entityMapping;
		}
	}
}
