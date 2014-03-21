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
        private readonly IDictionary<Type,IEntityMappingProvider> _parentProviders;

        public InheritanceTreeProvider(IEntityMappingProvider mainProvider,IEnumerable<IEntityMappingProvider> parentProviders)
        {
            _mainProvider=mainProvider;
            _parentProviders=parentProviders.ToDictionary(mp => mp.EntityType,mp => mp);
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
                return _mainProvider.Classes.Union(_parentProviders.SelectMany(p => p.Value.Classes));
            }
        }

        public IEnumerable<IPropertyMappingProvider> Properties
        {
            get
            {
                var providers=new Dictionary<string,IPropertyMappingProvider>();
                foreach (var property in _mainProvider.Properties)
                {
                    providers[property.PropertyInfo.Name]=property;
                }

                var parents=new Queue<Type>(_mainProvider.EntityType.GetImmediateParents());
                while (parents.Any())
                {
                    var iface=parents.Dequeue();
                    foreach (var immediateParent in iface.GetImmediateParents())
                    {
                        parents.Enqueue(immediateParent);
                    }

                    if (_parentProviders.ContainsKey(iface))
                    {
                        var provider=_parentProviders[iface];
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

        public void Accept(IMappingProviderVisitor visitor)
        {
        }

        private class PropertyInfoComparer:IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x,PropertyInfo y)
            {
                if (x==null||y==null)
                {
                    return false;
                }

                if (x.DeclaringType!=null&&x.DeclaringType.IsGenericType)
                {
                    x=x.DeclaringType.GetGenericTypeDefinition().GetProperty(x.Name);
                }

                if (y.DeclaringType!=null&&y.DeclaringType.IsGenericType)
                {
                    y=y.DeclaringType.GetGenericTypeDefinition().GetProperty(y.Name);
                }

                return object.Equals(x,y);
            }

            public int GetHashCode(PropertyInfo property)
            {
                if (property.DeclaringType!=null&&property.DeclaringType.IsGenericType)
                {
                    property=property.DeclaringType.GetGenericTypeDefinition().GetProperty(property.Name);
                }

                return property.GetHashCode();
            }
        }
    }
}