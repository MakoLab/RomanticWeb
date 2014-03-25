using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Providers;
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

            _mappingsRepository = new MappingsRepository();
            foreach (var mappingSource in CreateMappingSources())
            {
                MappingsRepository.AddSource(GetType().Assembly, mappingSource);
            }

            IEnumerable<IConvention> conventions = new IConvention[]
                                                       {
                                                           new DefaultDictionaryKeyPredicateConvention(),
                                                           new DefaultDictionaryValuePredicateConvention(),
                                                           new CollectionStorageConvention(),
                                                           new RdfListConvention()
                                                       };
            MappingsRepository.RebuildMappings(new MappingContext(_ontologies.Object, conventions));
        }

        protected abstract IEnumerable<IMappingProviderSource> CreateMappingSources();

        private static Uri GetUri(string prefix, string term)
        {
            return new Uri("http://example/livingThings#" + term);
        }
    }
}