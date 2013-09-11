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

        public IEntityFactory EntityFactory { get; private set; }

        [SetUp]
        public void Setup()
        {
            _store = new TripleStore();
            EntityFactory = new EntityFactory(_store, new StaticOntologyProvider());
        }

        protected void LoadTestFile(string fileName)
        {
            _store.LoadTestFile("TriplesWithLiteralSubjects.ttl");
        }
    }
}