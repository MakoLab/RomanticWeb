using System;
using Microsoft.CSharp.RuntimeBinder;
using Moq;
using NUnit.Framework;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.dotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityFactoryTests
    {
        private IEntityFactory _entityFactory;
        private IOntologyProvider _ontologyProvider;
        private Mock<IMappingsRepository> _mappings;
        private ITripleStore _store;

        [SetUp]
        public void Setup()
        {
            _ontologyProvider = new StaticOntologyProvider();
            _store = new TripleStore();
            _mappings = new Mock<IMappingsRepository>(MockBehavior.Strict);
            var tripleSourceFactory = new TripleStoreTripleSourceFactory(_store);
            _entityFactory = new EntityFactory(_mappings.Object, _ontologyProvider, tripleSourceFactory);
        }

        [Test]
        public void Creating_new_Entity_should_create_an_instance_with_id()
        {
            // when
            dynamic entity = _entityFactory.Create(new UriId("http://magi/people/Tomasz"));

            // when
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity, Is.TypeOf<Entity>());
            Assert.That(entity.Id, Is.EqualTo(new UriId("http://magi/people/Tomasz")));
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void Creating_new_Entity_should_throw_when_EntityId_is_null()
        {
            _entityFactory.Create((EntityId)null);
        }

        [Test]
        public void Creating_new_Entity_should_add_getters_for_known_ontology_namespaces()
        {
            // when
            dynamic entity = _entityFactory.Create(new UriId("http://magi/people/Tomasz"));
  
            // then
            Assert.That(entity.foaf, Is.Not.Null);
            Assert.That(entity.foaf, Is.InstanceOf<OntologyAccessor>());
        }

        [Test, ExpectedException(typeof(RuntimeBinderException))]
        public void Creating_new_Entity_should_not_add_getters_for_any_other_ontology_namespaces()
        {
            // given
            dynamic entity = _entityFactory.Create(new UriId("http://magi/people/Tomasz"));

            // when
            var accessor = entity.dcterms;
        }
    }
}
