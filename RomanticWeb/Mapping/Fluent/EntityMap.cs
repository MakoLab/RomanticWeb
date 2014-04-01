using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Base class for fluently defining entity mappings
    /// </summary>
    public abstract class EntityMap<TEntity> : EntityMap
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMap{TEntity}"/> class.
        /// </summary>
		protected EntityMap()
			: base(typeof(TEntity))
		{
		}

        /// <summary>
        /// Gets a builder for mapping a property
        /// </summary>
		protected IPropertyMap Property<TReturnType>(Expression<Func<TEntity,TReturnType>> prop)
		{
			var propertyMap = new PropertyMap(prop.ExtractPropertyInfo());

			MappedProperties.Add(propertyMap);

			return propertyMap;
		}

        /// <summary>
        /// Gets a builder for mapping a collection property
        /// </summary>
        protected ICollectionMap Collection<TReturnType>(Expression<Func<TEntity,TReturnType>> prop)
        {
            var propertyMap=new CollectionMap(prop.ExtractPropertyInfo());

			MappedProperties.Add(propertyMap);

			return propertyMap;
		}

        /// <summary>
        /// Gets a builder for mapping a dictionary property
        /// </summary>
        protected IDictionaryMap Dictionary<TReturnType>(Expression<Func<TEntity,TReturnType>> prop)
        {
            var dictionaryMap = new DictionaryMap(prop.ExtractPropertyInfo());

            MappedProperties.Add(dictionaryMap);

            return dictionaryMap;
        }
	}

    /// <summary>
    /// Base class for fluently defining entity entityMappings
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic and non-generic flavour of same class")]
    public abstract class EntityMap
    {
        private readonly Type _type;
        private readonly IList<ClassMap> _classes;
        private readonly IList<PropertyMapBase> _mappedProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMap"/> class.
        /// </summary>
        /// <param name="type">The mapped type.</param>
        protected EntityMap(Type type)
        {
            _type=type;
            _mappedProperties=new List<PropertyMapBase>();
            _classes=new List<ClassMap>();
        }

        internal Type Type
        {
            get
            {
                return _type;
            }
        }

        internal IEnumerable<ClassMap> Classes
        {
            get
            {
                return _classes;
            }
        }

        internal IEnumerable<PropertyMapBase> Properties
        {
            get
            {
                return _mappedProperties;
            }
        }

        internal IList<PropertyMapBase> MappedProperties
        {
            get
            {
                return _mappedProperties;
            }
        }

        /// <summary>
        /// Gets a builder for mapping the type
        /// </summary>
        protected ClassMap Class
        {
            get
            {
                var classMap=new ClassMap();
                _classes.Add(classMap);
                return classMap;
            }
        }

        /// <summary>
        /// Accepts the specified fluent maps visitor.
        /// </summary>
        /// <param name="fluentMapsVisitor">The fluent maps visitor.</param>
        public IEntityMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}
