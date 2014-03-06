using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository:MappingsRepositoryBase
    {
        private readonly List<EntityMap> _entityMaps;

        public TestMappingsRepository(params EntityMap[] entityMaps)
        {
            _entityMaps = entityMaps.ToList();
        }

        ////public void RebuildMappings(MappingContext mappingContext)
        ////{
        ////    _mappingContext = mappingContext;
        ////    _mappings=_entityMaps.Select(m => m.CreateMapping(mappingContext)).ToList();
        ////}

        ////public IEntityMapping MappingFor<TEntity>()
        ////{
        ////    return MappingFor(typeof(TEntity));
        ////}

        ////public IEntityMapping MappingFor(Type entityType)
        ////{
        ////    return (from mapping in Mappings
        ////            where mapping.EntityType==entityType
        ////            select mapping).SingleOrDefault();
        ////}

        ////public Type MappingFor(Uri classUri)
        ////{
        ////    return _entityMaps.Where(map => 
        ////        {
        ////            return map.Classes.Any(item =>
        ////            {
        ////                Uri uri=item.TermUri;
        ////                if (uri==null)
        ////                {
        ////                    Ontology ontology=_mappingContext.OntologyProvider.Ontologies.Where(element => element.Prefix==item.NamespacePrefix).FirstOrDefault();
        ////                    if (ontology!=null)
        ////                    {
        ////                        uri=new Uri(ontology.BaseUri.AbsoluteUri+item.TermName);
        ////                    }
        ////                }

        ////                return (uri!=null)&&(uri.AbsoluteUri==classUri.AbsoluteUri);
        ////            });
        ////        }).Select(map => map.EntityType).FirstOrDefault();
        ////}

        public void Add(EntityMap personMapping)
        {
            _entityMaps.Add(personMapping);
        }

        protected override IEnumerable<IEntityMapping> CreateMappings(MappingContext mappingContext)
        {
            return _entityMaps.Select(m => m.CreateMapping(mappingContext));
        }
    }
}