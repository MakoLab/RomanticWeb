using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository:IMappingsRepository
    {
        private readonly List<EntityMap> _entityMaps;

        private MappingContext _mappingContext;

        public TestMappingsRepository(params EntityMap[] entityMaps)
        {
            _entityMaps=entityMaps.ToList();
        }

        public IEntityMapping MappingFor<TEntity>()
        {
            return MappingFor(typeof(TEntity));
        }

        public void RebuildMappings(MappingContext mappingContext)
        {
            _mappingContext=mappingContext;
        }

        public IEntityMapping MappingFor(Type entityType)
        {
            var entityMap = _entityMaps.FirstOrDefault(map => map.EntityType == entityType);

            if (entityMap == null)
            {
                throw new MappingException(string.Format("Mapping not found for type {0}", entityType));
            }

            return entityMap.CreateMapping(_mappingContext);
        }

        public void Add(EntityMap personMapping)
        {
            _entityMaps.Add(personMapping);
        }
    }
}