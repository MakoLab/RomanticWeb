using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private EntityStore _entityStore;
        private IEntityContext _entityContext;
        private IEntityContextFactory _factory;

        public virtual bool IncludeFoaf
        {
            get
            {
                return false;
            }
        }

        public IMappingsRepository Mappings { get; private set; }

        protected IEntityContext EntityContext
        {
            get
            {
                if (_entityContext==null)
                {
                    _entityContext= Factory.CreateContext();
                }

                return _entityContext;
            }
        }

        protected IEntityStore EntityStore
        {
            get
            {
                return _entityStore;
            }
        }

        protected EntityContextFactory Factory
        {
            get
            {
                return (EntityContextFactory)_factory;
            }
        }

        [SetUp]
        public void Setup()
        {
            Mappings=SetupMappings();
            _entityStore=new EntityStore();

            _factory=new EntityContextFactory().WithEntitySource(CreateEntitySource)
                                               .WithOntology(new DefaultOntologiesProvider())
                                               .WithOntology(new TestOntologyProvider(IncludeFoaf))
                                               .WithMappings(m=>m.AddMapping(GetType().Assembly,Mappings))
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
            mock.Setup(m => m.RebuildMappings(It.IsAny<MappingContext>()));
            return mock.Object;
        }

        protected virtual void ChildSetup()
        {
        }

        protected abstract void LoadTestFile(string fileName);

        protected abstract IEntitySource CreateEntitySource();
    }
}