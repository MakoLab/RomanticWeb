using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Sources;

namespace RomanticWeb.Tests.Stubs
{
    public class TestMappingSource : IMappingProviderSource
    {
        private readonly RomanticWeb.Mapping.Visitors.IFluentMapsVisitor _builder = new FluentMappingProviderBuilder();
        private readonly List<IEntityMappingProvider> _entityMaps;

        public TestMappingSource(params EntityMap[] entityMaps)
        {
            _entityMaps = new List<IEntityMappingProvider>(entityMaps.Length);
            foreach (var entityMap in entityMaps)
            {
                Add(entityMap);
            }
        }

        public string Description
        {
            get
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        public IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            return _entityMaps;
        }

        public void Add(EntityMap entityMap)
        {
            _entityMaps.Add(entityMap.Accept(_builder));
        }
    }
}