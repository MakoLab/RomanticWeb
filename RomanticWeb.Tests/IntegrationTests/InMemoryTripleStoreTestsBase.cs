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

        public IEntityFactory EntityFactory { get; private set; }

        [SetUp]
        public void Setup()
        {
            _store = new TripleStore();
            _mappings = new Mock<IMappingProvider>(MockBehavior.Strict);
            var tripleSourceFactory = new TripleStoreTripleSourceFactory(_store);
            EntityFactory = new EntityFactory(_mappings.Object, new StaticOntologyProvider(), tripleSourceFactory);
        }

        protected void LoadTestFile(string fileName)
        {
            _store.LoadTestFile(fileName);
        }
    }
}