using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Builder for registering mapping repositories with <see cref="IEntityContextFactory"/>
    /// </summary>
    public sealed class MappingBuilder
    {
        private readonly IDictionary<Tuple<Assembly,Type>, IMappingsRepository> _mappingsRepositories=new Dictionary<Tuple<Assembly,Type>,IMappingsRepository>(); 

        /// <summary>
        /// Allows registering attribute mappings
        /// </summary>
        public MappingFrom Attributes
        {
            get
            {
                return new MappingFromAttributes(this);
            }
        }

        /// <summary>
        /// Allows registering fluent mappings
        /// </summary>
        public MappingFrom Fluent
        {
            get
            {
                return new MappingFromFluent(this);
            }
        }

        internal void AddMapping<TMappingRepository>(Assembly mappingAssembly,TMappingRepository mappingsRepository)
            where TMappingRepository:IMappingsRepository
        {
            var key=Tuple.Create(mappingAssembly,typeof(TMappingRepository));

            if (!_mappingsRepositories.ContainsKey(key))
            {
                _mappingsRepositories.Add(key,mappingsRepository);
            }
        }

        internal IEnumerable<IMappingsRepository> BuildMappings(Action<MappingBuilder> buildDelegate)
        {
            var currentRepositories=_mappingsRepositories.Values.ToList();
            buildDelegate.Invoke(this);
            return _mappingsRepositories.Values.Except(currentRepositories);
        }
    }
}