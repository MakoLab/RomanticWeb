using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Mapping repository, which scans an assembly for fluent mapping classes 
    /// </summary>
    public sealed class FluentMappingsRepository : AssemblyMappingsRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="FluentMappingsRepository"/>
        /// </summary>
        public FluentMappingsRepository(Assembly assembly):base(assembly)
        {
        }

        /// <summary>
        /// Scans the <see cref="AssemblyMappingsRepository.Assembly"/> for implementations of
        /// <see cref="EntityMap"/> and uses them to build mappings
        /// </summary>
        protected override IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(MappingContext mappingContext)
        {
            var maps=(from type in Assembly.GetTypes()
                      where typeof(EntityMap).IsAssignableFrom(type)
                      let map = (EntityMap)Activator.CreateInstance(type)
                      select new Tuple<Type,EntityMapping>(map.EntityType,map.CreateMapping(mappingContext))).ToList();

            foreach (var tuple in maps)
            {
                var currentMap=tuple;
                var parentMaps=maps.Where(map => map.Item1.IsAssignableFrom(currentMap.Item1));

                var missingInheritedProperties=from parentMap in parentMaps
                                               from property in parentMap.Item2.Properties
                                               where currentMap.Item2.Properties.All(p => p.Name!=property.Name)
                                               select property;

                foreach (var missingProperty in missingInheritedProperties)
                {
                    currentMap.Item2.Properties.Add(missingProperty);
                }

                yield return new Tuple<Type, IEntityMapping>(tuple.Item1, tuple.Item2);
            }
        }
    }
}