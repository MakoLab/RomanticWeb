using NUnit.Framework;
using RomanticWeb.Tests.Helpers;
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
            _store.LoadTestFile("TriplesWithLiteralSubjects.ttl");

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
            _store.LoadTestFile("TriplesWithLiteralSubjects.ttl");

            // when
            dynamic tomasz = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.nick == null);
        }

        [Test]
        public void Creating_Entity_should_allow_associated_Entity_subjects()
        {
            // given
            _store.LoadTestFile("AssociatedInstances.ttl");

            // when
            dynamic tomasz = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.knows, Is.TypeOf<Entity>());
            Assert.That(tomasz.foaf.knows.Id, Is.EqualTo(new EntityId("http://magi/people/Karol")));
        }

        [Test]
        public void Associated_Entity_should_allow_fetching_its_properties()
        {
            // given
            _store.LoadTestFile("AssociatedInstances.ttl");

            // when
            dynamic tomasz = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));
            dynamic karol = tomasz.foaf.knows;

            // then
            Assert.That(karol.foaf.givenName, Is.EqualTo("Karol"));
        }
    }
}