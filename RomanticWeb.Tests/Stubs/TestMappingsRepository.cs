using System.Collections.Generic;
using System.Linq;

using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository : IMappingsRepository
    {
        private readonly List<EntityMap> _entityMaps;

        public TestMappingsRepository(params EntityMap[] entityMaps)
        {
            this._entityMaps = entityMaps.ToList();
        }

        public IMapping MappingFor<TEntity>()
        {
            return this._entityMaps.Where(map => map.EntityType == typeof(TEntity)).Cast<IMappingProvider>().First().GetMapping();
        }

        public void Add(EntityMap personMapping)
        {
            this._entityMaps.Add(personMapping);
        }
    }
}