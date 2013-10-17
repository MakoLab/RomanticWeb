using System;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private IMappingsRepository _mappings;

        private EntityStore _entityStore;

        public IMappingsRepository Mappings
        {
            get
            {
                return _mappings;
            }
        }

        protected IEntityContext EntityContext { get; private set; }

        protected EntityStore EntityStore
        {
            get
            {
                return _entityStore;
            }
        }

        [SetUp]
        public void Setup()
        {
            _mappings = SetupMappings();
            var ontologyProvider=new CompoundOntologyProvider(new DefaultOntologiesProvider(),new TestOntologyProvider());
            _entityStore=new EntityStore();
            EntityContext=new EntityContext(Mappings,EntityStore,CreateEntitySource())
                              {
                                  OntologyProvider=ontologyProvider
                              };

            ChildSetup();
        }

        [TearDown]
        public void Teardown()
        {
            ChildTeardown();
        }

        protected virtual void ChildTeardown()
        {
        }

        protected virtual IMappingsRepository SetupMappings()
        {
            var mock=new Mock<IMappingsRepository>();
            mock.Setup(m => m.RebuildMappings(It.IsAny<IOntologyProvider>()));
            return mock.Object;
        }

        protected virtual void ChildSetup()
        {
        }

        protected abstract void LoadTestFile(string fileName);

        protected abstract void LoadTestFile(string fileName, Uri graphUri);

        protected abstract IEntitySource CreateEntitySource();
    }
}