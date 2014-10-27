using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Sources;

namespace RomanticWeb.Mapping.Providers
{
    internal class InheritanceTreeProvider : VisitableEntityMappingProviderBase, IEntityMappingProviderWithHiddenProperties
    {
        private readonly IEntityMappingProvider _mainProvider;
        private readonly IDictionary<Type, IEntityMappingProvider> _parentProviders;
        private readonly List<IPropertyMappingProvider> _hiddenProperties = new List<IPropertyMappingProvider>();
        private readonly List<IPropertyMappingProvider> _propertyProviders = new List<IPropertyMappingProvider>();

        public InheritanceTreeProvider(IEntityMappingProvider mainProvider, IEnumerable<IEntityMappingProvider> parentProviders)
        {
            _mainProvider = mainProvider;
            _parentProviders = parentProviders.ToDictionary(mp => mp.EntityType, mp => mp);
            DiscoverProperties();
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
                return _propertyProviders;
            }
        }

        public IEnumerable<IPropertyMappingProvider> HiddenProperties
        {
            get
            {
                return _hiddenProperties;
            }
        }

        private void DiscoverProperties()
        {
            var inheritedProperties = new HashSet<IPropertyMappingProvider>();
            var properties = new Dictionary<string, IPropertyMappingProvider>();
            foreach (var property in _mainProvider.Properties)
            {
                properties[property.PropertyInfo.Name] = property;
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
                    foreach (var propertyMapping in provider.Properties)
                    {
                        inheritedProperties.Add(propertyMapping);
                    }
                }
            }

            _propertyProviders.AddRange(properties.Values);

            foreach (var inheritedProperty in inheritedProperties.GroupBy(p => p.PropertyInfo.Name))
            {
                if (properties.ContainsKey(inheritedProperty.Key))
                {
                    _hiddenProperties.AddRange(inheritedProperty.Where(p => p.PropertyInfo.DeclaringType.IsGenericTypeDefinition == false));
                }
                else
                {
                    var propertyMappingProvider = (from property in inheritedProperty
                                                   group property by property.PropertyInfo.DeclaringType.IsGenericTypeDefinition into g 
                                                   select g).ToDictionary(g => g.Key, g => g);

                    if (propertyMappingProvider[false].Any())
                    {
                        _propertyProviders.AddRange(propertyMappingProvider[false]);
                    }
                    else
                    {
                        _propertyProviders.AddRange(propertyMappingProvider[true]); 
                    }
                }
            }
        }
    }
}