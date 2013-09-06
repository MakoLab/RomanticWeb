using NUnit.Framework;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.dotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class InMemoryTripleStoreTests
    {
        private IEntityFactory _entityFactory;
        private TripleStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new TripleStore();
            _entityFactory = new EntityFactory(_store, new StaticOntologyProvider());
        }

        [Test]
        public void Creating_Entity_should_allow_accessing_existing_literal_properties()
        {
            // given
            _store.LoadFromEmbeddedResource("RomanticWeb.Tests.TestGraphs.TriplesWithLiteralSubjects.ttl, RomanticWeb.Tests");

            // when
            dynamic tomasz = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(tomasz.foaf.familyName, Is.EqualTo("Pluskiewicz"));
            Assert.That(tomasz.foaf.nick == null);
        }

        [Test]
        public void Creating_Entity_should_return_null_when_triple_doesnt_exist()
        {
            // given
            _store.LoadFromEmbeddedResource("RomanticWeb.Tests.TestGraphs.TriplesWithLiteralSubjects.ttl, RomanticWeb.Tests");

            // when
            dynamic tomasz = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.nick == null);
        }
    }
}