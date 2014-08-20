using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.ComponentModel;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Mapping
{
    public abstract class MappingRepositoryTestBase
    {
        private Mock<IOntologyProvider> _ontologies;
        private MappingsRepository _mappingsRepository;

        protected MappingsRepository MappingsRepository
        {
            get
            {
                return _mappingsRepository;
            }
        }

        [SetUp]
        public void Setup()
        {
            _ontologies = new Mock<IOntologyProvider>();
            _ontologies.Setup(o => o.ResolveUri(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns((string p, string t) => GetUri(p, t));
            IServiceContainer container = new ServiceContainer();

            container.RegisterFrom<ConventionsCompositionRoot>();
            var conventions = container.GetInstance<IEnumerable<IConvention>>();

            _mappingsRepository = new MappingsRepository(
                new MappingContext(_ontologies.Object, conventions),
                CreateMappingSources(),
                new IMappingProviderVisitor[0], 
                new IMappingModelVisitor[0]);
        }

        protected abstract IEnumerable<IMappingProviderSource> CreateMappingSources();

        private static Uri GetUri(string prefix, string term)
        {
            return new Uri("http://example/livingThings#" + term);
        }
    }
}