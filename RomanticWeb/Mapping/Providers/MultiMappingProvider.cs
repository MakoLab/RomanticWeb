using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    internal class MultiMappingProvider:IEntityMappingProvider
    {
        private readonly Type _entityType;
        private readonly IEnumerable<IEntityMappingProvider> _entityMappingProviders;

        public MultiMappingProvider(Type entityType,IEnumerable<IEntityMappingProvider> entityMappingProviders)
        {
            _entityType=entityType;
            _entityMappingProviders=entityMappingProviders;
        }

        public IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _entityMappingProviders.SelectMany(mp => mp.Classes);
            }
        }

        public IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                return _entityMappingProviders.SelectMany(mp => mp.Properties);
            }
        }

        public Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        public void Accept(IMappingProviderVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}