using System;
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

        public Type MappingFor(Uri classUri)
        {
            return _entityMaps.Where(map => 
                {
                    return map.Classes.Any(item =>
                    {
                        Uri uri=item.TermUri;
                        if (uri==null)
                        {
                            Ontology ontology=_mappingContext.OntologyProvider.Ontologies.Where(element => element.Prefix==item.NamespacePrefix).FirstOrDefault();
                            if (ontology!=null)
                            {
                                uri=new Uri(ontology.BaseUri.AbsoluteUri+item.TermName);
                            }
                        }

                        return (uri!=null)&&(uri.AbsoluteUri==classUri.AbsoluteUri);
                    });
                }).Select(map => map.EntityType).FirstOrDefault();
        }

        public void Add(EntityMap personMapping)
        {
            _entityMaps.Add(personMapping);
        }
    }
}