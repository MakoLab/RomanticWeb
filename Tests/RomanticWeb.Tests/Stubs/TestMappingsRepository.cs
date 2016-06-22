using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Sources;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingsRepository : IMappingsRepository
    {
        private readonly List<IEntityMapping> _mappings;

        public TestMappingsRepository(params IEntityMapping[] entityMaps)
        {
            _mappings = entityMaps.ToList();
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

        public IPropertyMapping MappingForProperty(Uri predicateUri)
        {
            throw new NotImplementedException();
        }

        public void AddSource(Assembly assembly, IMappingProviderSource mappingProviderSource)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IEntityMapping> GetEnumerator()
        {
            return _mappings.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Add(IEntityMapping mapping)
        {
            _mappings.Add(mapping);
        }
    }
}