using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Providers
{
    internal class InheritanceMappingBuilder
    {
        private readonly IList<IEntityMappingProvider> _originalMappings;

        public InheritanceMappingBuilder(IList<IEntityMappingProvider> originalMappings)
        {
            _originalMappings=originalMappings;
        }

        public IEnumerable<IEntityMappingProvider> CombineInheritingMappings()
        {
            foreach (var mapping in _originalMappings)
            {
                var parentTypesMappings=GetParentMappings(mapping).ToList();

                if (parentTypesMappings.Any())
                {
                    yield return new InheritanceTreeProvider(mapping,parentTypesMappings);
                }
                else
                {
                    yield return mapping;
                }
            }
        }

        private IEnumerable<IEntityMappingProvider> GetParentMappings(IEntityMappingProvider childMapping)
        {
            return from m in _originalMappings
                   where m.EntityType != childMapping.EntityType
                   where IsDerivedFrom(childMapping.EntityType,m.EntityType)
                   select m;
        }

        private bool IsDerivedFrom(Type child,Type parent)
        {
            var anyInterfaceDerivesFromParent=(from iface in child.GetInterfaces()
                                               where iface.IsGenericType
                                               let genericDefinition = iface.GetGenericTypeDefinition()
                                               where genericDefinition==parent
                                               select iface).Any();

            return parent.IsAssignableFrom(child) || anyInterfaceDerivesFromParent;
        }
    }
}