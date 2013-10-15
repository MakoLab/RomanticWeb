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
        private IOntologyProvider _ontologies;
        private readonly List<EntityMap> _entityMaps;

        IOntologyProvider IMappingsRepository.OntologyProvider
        {
            get
            {
                return _ontologies;
            }
            set
            {
                _ontologies=value;
            }
        }

        public TestMappingsRepository(IOntologyProvider ontologies,params EntityMap[] entityMaps)
        {
            _ontologies=ontologies;
            this._entityMaps=entityMaps.ToList();
        }

        public IEntityMapping MappingFor<TEntity>()
        {
            return this._entityMaps.Where(map => map.EntityType==typeof(TEntity)).Cast<IMappingProvider>().First().CreateMapping(_ontologies);
        }

        public void Add(EntityMap personMapping)
        {
            this._entityMaps.Add(personMapping);
        }
    }
}