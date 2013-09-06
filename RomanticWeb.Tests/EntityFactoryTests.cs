using System;
using NUnit.Framework;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.dotNetRDF;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityFactoryTests
    {
        private IEntityFactory _entityFactory;
        private IOntologyProvider _ontologyProvider;

        [SetUp]
        public void Setup()
        {
            _ontologyProvider = new StaticOntologyProvider();
            _entityFactory = new EntityFactory(_ontologyProvider);
        }

        [Test]
        public void Creating_new_Entity_should_create_an_instance_with_id()
        {
            // when
            dynamic entity = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));

            // when
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity, Is.TypeOf<Entity>());
            Assert.That(entity.Id, Is.EqualTo(new EntityId("http://magi/people/Tomasz")));
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void Creating_new_Entity_should_throw_when_EntityId_is_null()
        {
            _entityFactory.Create(null);
        }

        [Test]
        public void Creating_new_Entity_should_add_getters_for_known_ontology_namespaces()
        {
            // when
            dynamic entity = _entityFactory.Create(new EntityId("http://magi/people/Tomasz"));
  
            // then
            Assert.That(entity.foaf, Is.Not.Null);
            Assert.That(entity.foaf, Is.TypeOf<IPredicateAccessor>());
        }
    }
}
