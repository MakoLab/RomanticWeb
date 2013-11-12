using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository:IMappingsRepository
    {
        private readonly List<EntityMap> _entityMaps;
        private IOntologyProvider _ontologies;

        public TestMappingsRepository(params EntityMap[] entityMaps)
        {
            _entityMaps=entityMaps.ToList();
        }

        public IEntityMapping MappingFor<TEntity>()
        {
            return _entityMaps.First(map => map.EntityType==typeof(TEntity)).CreateMapping(_ontologies);
        }

        public void RebuildMappings(IOntologyProvider ontologyProvider)
        {
            _ontologies=ontologyProvider;
        }

        public void Add(EntityMap personMapping)
        {
            _entityMaps.Add(personMapping);
        }
    }
}