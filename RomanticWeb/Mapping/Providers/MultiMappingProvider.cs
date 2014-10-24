using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Providers
{
    internal class MultiMappingProvider : VisitableEntityMappingProviderBase
    {
        private readonly Type _entityType;
        private readonly IEnumerable<IEntityMappingProvider> _entityMappingProviders;

        public MultiMappingProvider(Type entityType, IEnumerable<IEntityMappingProvider> entityMappingProviders)
        {
            _entityType = entityType;
            _entityMappingProviders = entityMappingProviders;
        }

        public override IEnumerable<IClassMappingProvider> Classes { get { return _entityMappingProviders.SelectMany(mp => mp.Classes); } }

        public override IEnumerable<IPropertyMappingProvider> Properties { get { return _entityMappingProviders.SelectMany(mp => mp.Properties); } }

        public override Type EntityType { get { return _entityType; } }
    }
}