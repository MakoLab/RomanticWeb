using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    internal class InheritanceTreeProvider:IEntityMappingProvider
    {
        private readonly IEntityMappingProvider _mainProvider;
        private readonly IList<IEntityMappingProvider> _parentProviders;

        public InheritanceTreeProvider(IEntityMappingProvider mainProvider,IEnumerable<IEntityMappingProvider> parentProviders)
        {
            _mainProvider=mainProvider;
            _parentProviders=parentProviders.ToList();
        }

        public Type EntityType
        {
            get
            {
                return _mainProvider.EntityType;
            }
        }

        public IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _mainProvider.Classes.Union(_parentProviders.SelectMany(p => p.Classes));
            }
        }

        public IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                var providers=new Dictionary<PropertyInfo,IPropertyMappingProvider>();
                foreach (var property in _mainProvider.Properties)
                {
                    providers[property.PropertyInfo]=property;
                }

                foreach (var property in _parentProviders.SelectMany(p => p.Properties))
                {
                    if (!providers.ContainsKey(property.PropertyInfo))
                    {
                        providers[property.PropertyInfo]=property;
                    }
                }

                return providers.Values;
            }
        }

        public void Accept(IMappingProviderVisitor visitor)
        {
        }
    }
}