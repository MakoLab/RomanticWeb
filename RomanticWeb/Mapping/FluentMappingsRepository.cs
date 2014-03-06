using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Mapping repository, which scans an assembly for fluent mapping classes 
    /// </summary>
    public sealed class FluentMappingsRepository : AssemblyMappingsRepository
    {
        private static readonly IEqualityComparer<Uri> UriComparer = new AbsoluteUriComparer();

        /// <summary>
        /// Creates a new instance of <see cref="FluentMappingsRepository"/>
        /// </summary>
        public FluentMappingsRepository(Assembly assembly):base(assembly)
        {
            LogTo.Trace("Created fluent mappings repository for assembly {0}",assembly);
        }

        /// <summary>
        /// Scans the <see cref="AssemblyMappingsRepository.Assembly"/> for implementations of
        /// <see cref="EntityMap"/> and uses them to build mappings
        /// </summary>
        protected override IEnumerable<IEntityMapping> CreateMappings(MappingContext mappingContext)
        {
            var maps=(from type in Assembly.GetTypes()
                      where type.IsConstructableEntityMap()
                      let map = (EntityMap)Activator.CreateInstance(type,true)
                      select map.CreateMapping(mappingContext)).ToList();

            foreach (var mapping in maps)
            {
                var currentMap=mapping;
                var parentMaps=maps.Where(map => map.EntityType.IsAssignableFrom(currentMap.EntityType)).ToList();

                FillMissingPropertyMappings(parentMaps,currentMap);
                FillMissingClassMappings(parentMaps,currentMap);

                yield return mapping;
            }
        }

        private static void FillMissingPropertyMappings(IEnumerable<EntityMapping> parentMaps,EntityMapping currentMap)
        {
            var missingInheritedProperties=from parentMap in parentMaps
                                           from property in parentMap.Properties
                                           where currentMap.Properties.All(p => p.Name!=property.Name)
                                           select property;

            foreach (var missingProperty in missingInheritedProperties)
            {
                currentMap.AddPropertyMapping(missingProperty);
            }
        }

        private static void FillMissingClassMappings(IEnumerable<EntityMapping> parentMaps,EntityMapping currentMap)
        {
            var missingInheritedClasses=from parentMap in parentMaps
                                        from classMap in parentMap.Classes
                                        where currentMap.Classes.All(c => !UriComparer.Equals(c.Uri,classMap.Uri))
                                        select classMap;

            foreach (var missingClass in missingInheritedClasses)
            {
                currentMap.AddClassMapping(missingClass);
            }
        }
    }
}