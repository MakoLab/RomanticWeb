using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Sources;

namespace RomanticWeb.Mapping.Providers
{
    internal class InheritanceTreeProvider : VisitableEntityMappingProviderBase
    {
        private readonly IEntityMappingProvider _mainProvider;
        private readonly IDictionary<Type, IEntityMappingProvider> _parentProviders;
        private ICollection<IPropertyMappingProvider> _propertyProviders;

        public InheritanceTreeProvider(IEntityMappingProvider mainProvider, IEnumerable<IEntityMappingProvider> parentProviders)
        {
            _mainProvider = mainProvider;
            _parentProviders = parentProviders.ToDictionary(mp => mp.EntityType, mp => mp);
        }

        public override Type EntityType
        {
            get
            {
                return _mainProvider.EntityType;
            }
        }

        public override IEnumerable<IClassMappingProvider> Classes
        {
            get
            {
                return _mainProvider.Classes.Union(_parentProviders.SelectMany(p => p.Value.Classes));
            }
        }

        public override IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                if (_propertyProviders == null)
                {
                    _propertyProviders = CombineProperties();
                }

                return _propertyProviders;
            }
        }

        private ICollection<IPropertyMappingProvider> CombineProperties()
        {
            var providers = new Dictionary<string, IPropertyMappingProvider>();
            foreach (var property in _mainProvider.Properties)
            {
                providers[property.PropertyInfo.Name] = property;
            }

            var parents = new Queue<Type>(_mainProvider.EntityType.GetImmediateParents());
            while (parents.Any())
            {
                var iface = parents.Dequeue();
                foreach (var immediateParent in iface.GetImmediateParents())
                {
                    parents.Enqueue(immediateParent);
                    if (immediateParent.IsGenericTypeDefinition && _parentProviders.ContainsKey(immediateParent))
                    {
                        var parentMapping = _parentProviders[immediateParent];
                        _parentProviders[iface] = new ClosedGenericEntityMappingProvider(
                            parentMapping, iface.GetGenericArguments());
                    }
                }

                if (_parentProviders.ContainsKey(iface))
                {
                    var provider = _parentProviders[iface];
                    foreach (var property in provider.Properties)
                    {
                        if (!providers.ContainsKey(property.PropertyInfo.Name))
                        {
                            providers[property.PropertyInfo.Name] = property;
                        }
                    }
                }
            }

            return providers.Values;
        }
    }
}