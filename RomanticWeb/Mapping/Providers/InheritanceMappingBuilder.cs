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
            return _originalMappings.Where(m => (m.EntityType.IsAssignableFrom(childMapping.EntityType))&&(m.EntityType!=childMapping.EntityType));
        }
    }
}