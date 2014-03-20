using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository : IMappingsRepository
    {
        private readonly IOntologyProvider _ontologyProvider;
        private readonly TestMappingSource _mappingSource;
        private MappingModelBuilder _builder;
        private List<IEntityMapping> _mappings;

        public TestMappingsRepository(IOntologyProvider ontologyProvider,params EntityMap[] entityMaps)
        {
            _ontologyProvider=ontologyProvider;
            _mappingSource=new TestMappingSource(entityMaps);
        }

        private List<IEntityMapping> Mappings
        {
            get
            {
                return _mappings;
            }
        }

        public void RebuildMappings(MappingContext mappingContext)
        {
            _builder=new MappingModelBuilder(mappingContext);
            _mappings=_mappingSource.GetMappingProviders().Select(m => _builder.BuildMapping(m)).ToList();
        }

        public IEntityMapping MappingFor<TEntity>()
        {
            return MappingFor(typeof(TEntity));
        }

        public IEntityMapping MappingFor(Type entityType)
        {
            return (from mapping in Mappings
                    where mapping.EntityType == entityType
                    select mapping).SingleOrDefault();
        }

        public Type MappingFor(Uri classUri)
        {
            return _mappingSource.GetMappingProviders().Where(map => map.Classes.Any(item =>
                {
                    Uri uri=item.GetTerm(_ontologyProvider);
                    return (uri != null) && (uri.AbsoluteUri == classUri.AbsoluteUri);
                })).Select(map => map.EntityType).FirstOrDefault();
        }

        public PropertyInfo MappingForProperty(Uri predicateUri)
        {
            return _mappingSource.GetMappingProviders().Select(map =>
            {
                var mapping = new
                {
                    Type = map.EntityType,
                    Property = map.Properties.FirstOrDefault(item =>
                        {
                            Uri uri=item.GetTerm(_ontologyProvider);
                            return (uri != null) && (uri.AbsoluteUri == predicateUri.AbsoluteUri);
                        })
                };

                return mapping.Property.PropertyInfo;
            }).SingleOrDefault();
        }
    }
}