using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    [DebuggerDisplay("Entity {EntityType}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    internal class EntityMapping : IEntityMapping
    {
        private readonly Type _entityType;
        private readonly List<PropertyMapping> _hiddenProperties;
        private readonly List<IClassMapping> _classes;
        private readonly List<PropertyMapping> _properties;

        internal EntityMapping(Type entityType, IEnumerable<IClassMapping> classes, IEnumerable<PropertyMapping> properties, IEnumerable<PropertyMapping> hiddenProperties)
            : this(classes, properties)
        {
            _entityType = entityType;
            _hiddenProperties = hiddenProperties.ToList();
            _properties.ToList().ForEach(p => p.EntityMapping = this);
        }

        internal EntityMapping(Type entityType)
            : this(entityType, new IClassMapping[0], new PropertyMapping[0], new PropertyMapping[0])
        {
        }

        protected EntityMapping(IEnumerable<IClassMapping> classes, IEnumerable<PropertyMapping> properties)
        {
            _entityType = typeof(IEntity);
            _properties = properties.ToList();
            _classes = classes.ToList();
        }

        public Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        public IEnumerable<IClassMapping> Classes { get { return _classes; } }

        public IEnumerable<IPropertyMapping> Properties
        {
            get
            {
                return _properties.Concat(_hiddenProperties);
            }
        }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            var propertyMappings = _properties.Where(p => p.Name == propertyName && !p.IsHidden).ToList();

            if (!propertyMappings.Any())
            {
                throw new MappingException(string.Format("No mapping found for property {0}", propertyName));
            }

            if (propertyMappings.Count > 1)
            {
                throw new AmbiguousPropertyException(propertyName);
            }

            return propertyMappings.Single();
        }

        public void Accept(IMappingModelVisitor mappingModelVisitor)
        {
            mappingModelVisitor.Visit(this);

            foreach (var propertyMapping in Properties)
            {
                propertyMapping.Accept(mappingModelVisitor);
            }

            foreach (var clas in Classes)
            {
                clas.Accept(mappingModelVisitor);
            }
        }

        private class DebuggerViewProxy
        {
            private readonly EntityMapping _mapping;

            public DebuggerViewProxy(EntityMapping mapping)
            {
                _mapping = mapping;
            }

            public Type EntityType
            {
                get
                {
                    return _mapping.EntityType;
                }
            }

            public IList<IClassMapping> Classes
            {
                get
                {
                    return _mapping.Classes.ToList();
                }
            }

            public IList<IPropertyMapping> Properties
            {
                get
                {
                    var propertyMappings = _mapping.Properties.ToList();
                    propertyMappings.Sort(CompareProperty);
                    return propertyMappings;
                }
            }

            private static int CompareProperty(IPropertyMapping left, IPropertyMapping right)
            {
                if (left.GetType() == right.GetType())
                {
                    return string.Compare(left.Name, right.Name, StringComparison.CurrentCulture);
                }

                if (left is CollectionMapping)
                {
                    return 1;
                }

                return -1;
            }
        }
    }
}