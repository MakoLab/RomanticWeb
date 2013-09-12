using Moq;
using NUnit.Framework;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.dotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    public class InMemoryTripleStoreTestsBase
    {
        private TripleStore _store;

        private Mock<IMappingProvider> _mappings;

        protected IEntityFactory EntityFactory { get; set; }

        [SetUp]
        public void Setup()
        {
            _store = new TripleStore();
            _mappings = SetupMappings();
            var tripleSourceFactory = new TripleStoreTripleSourceFactory(_store);
            EntityFactory = new EntityFactory(_mappings.Object, new StaticOntologyProvider(), tripleSourceFactory);

            ChildSetup();
        }

        [TearDown]
        public void Teardown()
        {
            _mappings.VerifyAll();
            ChildTeardown();
        }

        protected virtual void ChildTeardown()
        {
        }

        protected virtual Mock<IMappingProvider> SetupMappings()
        {
            return new Mock<IMappingProvider>(MockBehavior.Strict);
        }

        protected virtual void ChildSetup()
        {
        }

        protected void LoadTestFile(string fileName)
        {
            _store.LoadTestFile(fileName);
        }
    }
}