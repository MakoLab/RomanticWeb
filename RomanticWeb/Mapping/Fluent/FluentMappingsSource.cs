using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Mapping repository, which scans an assembly for fluent mapping classes 
    /// </summary>
    public sealed class FluentMappingsSource : AssemblyMappingsSource
    {
        /// <summary>
        /// Creates a new instance of <see cref="FluentMappingsSource"/>
        /// </summary>
        public FluentMappingsSource(Assembly assembly):base(assembly)
        {
            LogTo.Trace("Created fluent mappings repository for assembly {0}",assembly);
        }

        /// <summary>
        /// Finds all fluent <see cref="EntityMap"/>s in the <see cref="AssemblyMappingsSource.Assembly"/>
        /// and transforms them to <see cref="IEntityMappingProvider"/>s
        /// </summary>
        public override IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            Visitors.IFluentMapsVisitor visitor = new FluentMappingProviderBuilder();
            var maps = (from type in Assembly.GetTypes()
                        where type.IsConstructableEntityMap()
                        let map=(EntityMap)Activator.CreateInstance(type,true)
                        select map.Accept(visitor));

            return maps;
        }
    }
}