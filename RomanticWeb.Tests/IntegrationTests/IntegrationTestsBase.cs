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

        private IEntityContextFactory _factory;

        private IEntityContext _entityContext;

        public IMappingsRepository Mappings
        {
            get
            {
                return _mappings;
            }
        }

        protected IEntityContext EntityContext
        {
            get
            {
                if(_entityContext==null)
                {
                    _entityContext= _factory.CreateContext();
                }

                return _entityContext;
            }
        }

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
            _entityStore = new EntityStore();

            _factory = new EntityContextFactory().WithEntitySource(CreateEntitySource)
                                                 .WithOntology(new DefaultOntologiesProvider())
                                                 .WithOntology(new TestOntologyProvider())
                                                 .WithMappings(_mappings)
                                                 .WithEntityStore(() => _entityStore);
            ChildSetup();
        }

        [TearDown]
        public void Teardown()
        {
            ChildTeardown();
            _entityContext=null;
        }

        protected virtual void ChildTeardown()
        {
        }

        protected virtual IMappingsRepository SetupMappings()
        {
            var mock = new Mock<IMappingsRepository>();
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