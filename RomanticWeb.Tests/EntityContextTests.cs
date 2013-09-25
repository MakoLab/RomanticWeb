using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityContextTests
    {
        private IEntityContext _entityContext;
        private IOntologyProvider _ontologyProvider;
        private Mock<IMappingsRepository> _mappings;
        private Mock<IEntityStore> _entityStore;
        private Mock<IEntitySource> _store;

        private IEnumerable<Lazy<IEntity>> TypedAndUntypedEntities
        {
            get
            {
                Setup();

                yield return new Lazy<IEntity>(() => _entityContext.Create(new EntityId("http://magi/people/Tomasz")));
                yield return new Lazy<IEntity>(
                    () =>
                    {
                        _mappings.Setup(m => m.MappingFor<IPerson>()).Returns(new EntityMapping());
                        return _entityContext.Create<IPerson>(new EntityId("http://magi/people/Tomasz"));
                    });
            }
        }

        [SetUp]
        public void Setup()
        {
            _ontologyProvider = new TestOntologyProvider();
            _mappings = new Mock<IMappingsRepository>(MockBehavior.Strict);
            _entityStore = new Mock<IEntityStore>(MockBehavior.Strict);
            _store = new Mock<IEntitySource>();
            _entityContext = new EntityContext(_mappings.Object, _ontologyProvider, _entityStore.Object, _store.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mappings.VerifyAll();
            _entityStore.VerifyAll();
            _store.VerifyAll();
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        public void Creating_new_Entity_should_create_an_instance_with_id(Lazy<IEntity> lazyEntity)
        {
            // when
            dynamic entity = lazyEntity.Value;

            // when
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity, Is.InstanceOf<IEntity>());
            Assert.That(entity.Id, Is.EqualTo(new EntityId("http://magi/people/Tomasz")));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Creating_new_Entity_should_throw_when_EntityId_is_null()
        {
            _entityContext.Create((EntityId)null);
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        public void Creating_new_Entity_should_add_getters_for_known_ontology_namespaces(Lazy<IEntity> lazyEntity)
        {
            // when
            dynamic entity = lazyEntity.Value.AsDynamic();

            // then
            Assert.That(entity.foaf, Is.Not.Null);
            Assert.That(entity.foaf, Is.InstanceOf<OntologyAccessor>());
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void Creating_new_Entity_should_not_add_getters_for_any_other_ontology_namespaces(Lazy<IEntity> lazyEntity)
        {
            // given
            dynamic entity = lazyEntity.Value.AsDynamic();

            // when
            var accessor = entity.dcterms;
        }

        [Test]
        public void Created_typed_entity_should_be_equal_to_Entity_with_same_Id()
        {
            // given
            _mappings.Setup(m => m.MappingFor<IPerson>()).Returns(new EntityMapping());
            var entity = _entityContext.Create<IPerson>(new EntityId("http://magi/people/Tomasz"));
            var typed = new Entity(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.AreEqual(entity, typed);
            Assert.AreEqual(entity.GetHashCode(), typed.GetHashCode());
            _mappings.Verify(m => m.MappingFor<IPerson>(), Times.Once);
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        public void Accessing_entity_id_should_not_trigger_lazy_load(Lazy<IEntity> lazyEntity)
        {
            // given
            IEntity entity = lazyEntity.Value;
            dynamic dynEntity = entity.AsDynamic();

            // when
            var id = entity.Id;
            id = dynEntity.Id;

            // then
            _store.Verify(s => s.LoadEntity(It.IsAny<IEntityStore>(), It.IsAny<EntityId>()), Times.Never);
        }

        [Test]
        public void Accessing_entity_member_for_the_first_time_should_trigger_lazy_load()
        {
            // when
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>()))
                        .Returns(new RdfNode[0]);
            dynamic entity = _entityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // when
            var id = entity.foaf.givenName;

            // then
            _store.Verify(s => s.LoadEntity(It.IsAny<IEntityStore>(), new EntityId("http://magi/people/Tomasz")), Times.Once);
        }

        [Test]
        public void Accessing_typed_entity_member_for_the_first_time_should_trigger_lazy_load()
        {
            // when
            var mockingMapping = new Mock<IMapping>();
            mockingMapping.Setup(m => m.PropertyFor(It.IsAny<string>()))
                          .Returns((string prop) => GetMapping(prop));
            _mappings.Setup(m => m.MappingFor<IPerson>()).Returns(mockingMapping.Object);
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(),It.IsAny<Uri>()))
                        .Returns(new RdfNode[0]);
            var entity = _entityContext.Create<IPerson>(new EntityId("http://magi/people/Tomasz"));

            // when
            var name = entity.FirstName;
            var page = entity.Homepage;
            var interests = entity.Interests;

            // then
            _store.Verify(s => s.LoadEntity(It.IsAny<IEntityStore>(), new EntityId("http://magi/people/Tomasz")), Times.Once);
        }

        private static IPropertyMapping GetMapping(string propertyName)
        {
            return new TestPropertyMapping
                       {
                           Name=propertyName,
                           Uri=new Uri("http://unittest/"+propertyName),
                           UsesUnionGraph=false,
                           IsCollection=false
                       };
        }
    }
}
