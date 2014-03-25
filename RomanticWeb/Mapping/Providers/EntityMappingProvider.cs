using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    internal class EntityMappingProvider : IEntityMappingProvider
    {
        private readonly IEnumerable<IClassMappingProvider> _classes;
        private readonly IEnumerable<IPropertyMappingProvider> _properties;
        private readonly Type _entityType;

        internal EntityMappingProvider(
            Type entityType,
            IEnumerable<IClassMappingProvider> classes,
            IEnumerable<IPropertyMappingProvider> properties)
        {
            _entityType=entityType;
            _classes=classes;
            _properties=properties;
        }

        public Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        public IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _classes;
            }
        }

        public IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                return _properties;
            }
        }

        public void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            var @this=((IEntityMappingProvider)this);
            mappingProviderVisitor.Visit(this);

            foreach (var property in @this.Properties)
            {
                property.Accept(mappingProviderVisitor);
            }

            foreach (var classProvider in @this.Classes)
            {
                classProvider.Accept(mappingProviderVisitor);
            }
        }
    }
}