using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    internal class EntityMappingProvider : VisitableEntityMappingProviderBase
    {
        private readonly IEnumerable<IClassMappingProvider> _classes;
        private readonly IEnumerable<IPropertyMappingProvider> _properties;
        private readonly Type _entityType;

        internal EntityMappingProvider(
            Type entityType,
            IEnumerable<IClassMappingProvider> classes,
            IEnumerable<IPropertyMappingProvider> properties)
        {
            _entityType = entityType;
            _classes = classes;
            _properties = properties;
        }

        public override Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        public override IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _classes;
            }
        }

        public override IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                return _properties;
            }
        }
    }
}