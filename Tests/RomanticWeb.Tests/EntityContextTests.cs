using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using Moq;
using NUnit.Framework;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;

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
        private Mock<IEntityContextFactory> _factory;
        private PropertyMapping _typesMapping;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelector;
        private Mock<IDatasetChangesTracker> _changesTracker;

        private IEnumerable<Lazy<IEntity>> TypedAndUntypedEntities
        {
            get
            {
                Setup();

                yield return new Lazy<IEntity>(
                    () =>
                        {
                            _entityStore.Setup(
                                s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                                        .Returns(new Node[0]);
                            return _entityContext.Load<IEntity>(new EntityId("http://magi/people/Tomasz"));
                        });
                yield return new Lazy<IEntity>(
                    () =>
                        {
                            _entityStore.Setup(
                                s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                                        .Returns(new Node[0]);
                            _mappings.Setup(m => m.MappingFor(typeof(IPerson)))
                                     .Returns(new EntityMapping(typeof(IPerson)));
                            return _entityContext.Load<IPerson>(new EntityId("http://magi/people/Tomasz"));
                        });
            }
        }

        [SetUp]
        public void Setup()
        {
            _typesMapping = new TestPropertyMapping(
                typeof(ITypedEntity), typeof(IEnumerable<EntityId>), "Types", Rdf.type);
            _factory = new Mock<IEntityContextFactory>();
            _ontologyProvider = new TestOntologyProvider();
            _mappings = new Mock<IMappingsRepository>();
            _entityStore = new Mock<IEntityStore>();
            _entityStore.Setup(es => es.GetObjectsForPredicate(It.IsAny<EntityId>(), Rdf.type, It.IsAny<Uri>()))
                        .Returns(new Node[0]);
            _mappings.Setup(m => m.MappingFor<ITypedEntity>())
                     .Returns(new EntityMapping(typeof(ITypedEntity), new ClassMapping[0], new[] { _typesMapping }));
            _mappings.Setup(m => m.MappingFor(typeof(ITypedEntity)))
                     .Returns(new EntityMapping(typeof(ITypedEntity), new ClassMapping[0], new[] { _typesMapping }));
            _store = new Mock<IEntitySource>();
            _baseUriSelector = new Mock<IBaseUriSelectionPolicy>(MockBehavior.Strict);
            var mappingContext = new MappingContext(_ontologyProvider);
            var catalog = new TestTransformerCatalog();
            _changesTracker = new Mock<IDatasetChangesTracker>(MockBehavior.Strict);
            _entityContext = new EntityContext(
                _factory.Object,
                _mappings.Object,
                mappingContext,
                _entityStore.Object,
                _store.Object,
                _baseUriSelector.Object,
                new TestCache(),
                new DefaultBlankNodeIdGenerator(),
                catalog,
                new ImpromptuInterfaceCaster((entity, mapping) => new EntityProxy(entity, mapping, catalog, new TestGraphSelector()), _mappings.Object),
                _changesTracker.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _store.VerifyAll();
            _baseUriSelector.VerifyAll();
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

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Creating_new_Entity_should_throw_when_EntityId_is_null()
        {
            _entityContext.Load<IEntity>(null);
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        public void Creating_new_Entity_should_add_getters_for_known_ontology_namespaces(Lazy<IEntity> lazyEntity)
        {
            // given
            dynamic entity = lazyEntity.Value.AsDynamic();

            // when
            var foaf = entity.foaf;

            // then
            Assert.That(foaf, Is.Not.Null);
            Assert.That(foaf, Is.InstanceOf<OntologyAccessor>());
        }

        [Test]
        [TestCaseSource("TypedAndUntypedEntities")]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void Creating_new_Entity_should_not_add_getters_for_any_other_ontology_namespaces(
            Lazy<IEntity> lazyEntity)
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
            _mappings.Setup(m => m.MappingFor(typeof(IPerson))).Returns(new EntityMapping(typeof(IPerson)));
            var entity = _entityContext.Load<IPerson>(new EntityId("http://magi/people/Tomasz"));
            var typed = new Entity(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.AreEqual(entity, typed);
            Assert.AreEqual(entity.GetHashCode(), typed.GetHashCode());
            _mappings.Verify(m => m.MappingFor(typeof(IPerson)), Times.Once);
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
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                        .Returns(new Node[0]);
            dynamic entity = _entityContext.Load<IEntity>(new EntityId("http://magi/people/Tomasz")).AsDynamic();

            // when
            var id = entity.foaf.givenName;

            // then
            _store.Verify(
                s => s.LoadEntity(It.IsAny<IEntityStore>(), new EntityId("http://magi/people/Tomasz")), Times.Once);
        }

        [Test]
        public void Accessing_typed_entity_member_for_the_first_time_should_trigger_lazy_load()
        {
            // when
            var mockingMapping = new Mock<IEntityMapping>();
            mockingMapping.Setup(m => m.PropertyFor(It.IsAny<string>())).Returns((string prop) => GetMapping(prop));
            _mappings.Setup(m => m.MappingFor(typeof(IPerson))).Returns(mockingMapping.Object);
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                        .Returns(new Node[0]);
            var entity = _entityContext.Load<IPerson>(new EntityId("http://magi/people/Tomasz"));

            // when
            var name = entity.Interests;
            var page = entity.Interests;
            var interests = entity.Interests;

            // then
            _store.Verify(
                s => s.LoadEntity(It.IsAny<IEntityStore>(), new EntityId("http://magi/people/Tomasz")), Times.Once);
        }

        [Test]
        public void Loading_entity_twice_should_initialize_only_once()
        {
            // given
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                        .Returns(new Node[0]);
            var entity = _entityContext.Load<IEntity>(new EntityId("http://magi/people/Tomasz"));
            var name = entity.AsDynamic().foaf.givenName;
            entity = _entityContext.Load<IEntity>(new EntityId("http://magi/people/Tomasz"));

            // when
            var page = entity.AsDynamic().foaf.homePage;

            // then
            _store.Verify(
                s => s.LoadEntity(It.IsAny<IEntityStore>(), new EntityId("http://magi/people/Tomasz")), Times.Once);
        }

        [Test]
        public void New_entity_should_not_trigger_initialization()
        {
            // given
            _entityStore.Setup(s => s.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Uri>(), It.IsAny<Uri>()))
                        .Returns(new Node[0]);
            var entity = _entityContext.Create<IEntity>(new EntityId("http://magi/people/Tomasz"));

            // when
            var page = entity.AsDynamic().foaf.homePage;

            // then
            Assert.That(page, Is.Empty);
            _store.Verify(s => s.LoadEntity(It.IsAny<IEntityStore>(), It.IsAny<EntityId>()), Times.Never);
        }

        [Test]
        public void Should_be_possible_to_load_entity_without_checking_for_existence()
        {
            // when
            var entity = _entityContext.Load<IEntity>(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(entity, Is.Not.Null);
            _store.Verify(s => s.EntityExist(It.IsAny<EntityId>()), Times.Never);
        }

        [Test]
        public void Should_apply_changes_to_underlying_store_when_committing()
        {
            // given
            var datasetChanges = new DatasetChange[0];
            _entityStore.Setup(store => store.ResetState());

            // when
            _entityContext.Commit();

            // then
            _store.Verify(store => store.Commit(datasetChanges), Times.Once);
        }

        [Test]
        public void Deleting_entity_should_mark_for_deletion_in_changeset()
        {
            // given
            var entityId = new EntityId("urn:some:entityid");
            _entityStore.Setup(store => store.Delete(entityId, DeleteBehaviour.DoNothing));

            // when
            _entityContext.Delete(entityId, DeleteBehaviour.DoNothing);

            // then
            _entityStore.Verify(store => store.Delete(entityId, DeleteBehaviour.DoNothing), Times.Once);
        }

        [Test]
        public void Creating_entity_with_relative_Uri_should_make_the_Uri_absolute()
        {
            // given
            var entityId = new EntityId("some/relative/uri");
            _baseUriSelector.Setup(bus => bus.SelectBaseUri(It.IsAny<EntityId>()))
                            .Returns(new Uri("http://test.com/base/"));

            // when
            var entity = _entityContext.Create<IEntity>(entityId);

            // then
            entity.Id.Should().Be(new EntityId("http://test.com/base/some/relative/uri"));
            _baseUriSelector.Verify(bus => bus.SelectBaseUri(entityId), Times.Once);
        }

        [Test]
        public void Loading_entity_with_relative_Uri_should_make_the_Uri_absolute()
        {
            // given
            var entityId = new EntityId("some/relative/uri");
            _baseUriSelector.Setup(bus => bus.SelectBaseUri(It.IsAny<EntityId>()))
                            .Returns(new Uri("http://test.com/base/"));

            // when
            var entity = _entityContext.Load<IEntity>(entityId);

            // then
            entity.Id.Should().Be(new EntityId("http://test.com/base/some/relative/uri"));
            _baseUriSelector.Verify(bus => bus.SelectBaseUri(entityId), Times.Once);
        }

        [Test]
        public void Deleting_entity_with_relative_Uri_should_make_the_Uri_absolute()
        {
            // given
            var entityId = new EntityId("some/relative/uri");
            _baseUriSelector.Setup(bus => bus.SelectBaseUri(It.IsAny<EntityId>()))
                            .Returns(new Uri("http://test.com/base/"));
            _entityStore.Setup(store => store.Delete(It.IsAny<EntityId>(), DeleteBehaviour.DoNothing));

            // when
            _entityContext.Delete(entityId, DeleteBehaviour.DoNothing);

            // then
            _baseUriSelector.Verify(bus => bus.SelectBaseUri(entityId), Times.Once);
        }

        [Test]
        public void Committing_should_process_changes_and_pass_them_to_triple_source()
        {
            // given
            IEnumerable<DatasetChange> changes = new[] { new TestChange("urn:some:id", "urn:the:graph") };

            // when
            _entityContext.Commit();

            // then
            _store.Verify(store => store.Commit(changes));
        }

        [Test]
        public void Rollback_should_reset_entity_source()
        {
            // given
            _entityStore.Setup(s => s.Rollback());
            _changesTracker.Setup(s => s.Clear());

            // when
            _entityContext.Rollback();

            // then
            _entityStore.Verify(s => s.Rollback());
        }

        private static PropertyMapping GetMapping(string propertyName)
        {
            return new TestPropertyMapping(typeof(IEntity), typeof(int), propertyName, new Uri("http://unittest/" + propertyName));
        }

        private class TestChange : DatasetChange
        {
            public TestChange(EntityId entity, EntityId graph)
                : base(entity, graph)
            {
            }

            public override DatasetChange MergeWith(DatasetChange other)
            {
                throw new NotImplementedException();
            }
        }
    }
}
