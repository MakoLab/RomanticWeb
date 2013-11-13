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
            var entityMap=_entityMaps.FirstOrDefault(map => map.EntityType==typeof(TEntity));

            if (entityMap==null)
            {
                throw new MappingException(string.Format("Mapping not found for type {0}",typeof(TEntity)));
            }

            return entityMap.CreateMapping(_ontologies);
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