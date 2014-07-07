using System;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Entities
{
    [TestFixture]
    public class EntityProxyTests
    {
        private readonly EntityId _entityId = new EntityId("urn:test:entity");
        private EntityProxy _entityProxy;
        private Mock<IEntityMapping> _mapping;
        private Mock<IEntityContext> _context;
        private Mock<INamedGraphSelector> _graphSelector;

        [SetUp]
        public void Setup()
        {
            _mapping = new Mock<IEntityMapping>(MockBehavior.Strict);
            _context = new Mock<IEntityContext>(MockBehavior.Strict);
            _graphSelector = new Mock<INamedGraphSelector>();

            _context.Setup(c => c.Store).Returns(new EntityStore());
            _context.Setup(c => c.GraphSelector).Returns(_graphSelector.Object);
            _context.Setup(c => c.InitializeEnitity(It.IsAny<IEntity>()));
            _graphSelector.Setup(g => g.SelectGraph(It.IsAny<EntityId>(), It.IsAny<IEntityMapping>(), It.IsAny<IPropertyMapping>()))
                          .Returns(new Uri("urn:default:graph"));

            var entity = new Entity(_entityId, _context.Object);
            _entityProxy = new EntityProxy(entity, _mapping.Object, new TestTransformerCatalog());
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Should_allow_overriding_parameters_for_selecting_named_graph()
        {
            // given
            var idToUse = new EntityId("urn:actual:id");
            var entityMappingToUse = ImpromptuInterface.Dynamic.Builder.New().ActLike<IEntityMapping>();
            Mock<IPropertyMapping> mappingToUse = new Mock<IPropertyMapping>();
            mappingToUse.SetupGet(instance => instance.Uri).Returns(Rdf.subject);
            var propertyMapping = new Mock<IPropertyMapping>();
            propertyMapping.SetupGet(instance => instance.Uri).Returns(Rdf.predicate);
            _mapping.Setup(m => m.PropertyFor("property")).Returns(propertyMapping.Object);

            // when
            _entityProxy.OverrideGraphSelection(new OverridingGraphSelector(idToUse, entityMappingToUse, mappingToUse.Object));
            Impromptu.InvokeGet(_entityProxy, "property");

            // then
            _graphSelector.Verify(c => c.SelectGraph(idToUse, entityMappingToUse, mappingToUse.Object), Times.Once);
            _graphSelector.Verify(c => c.SelectGraph(_entityId, _mapping.Object, propertyMapping.Object), Times.Never);
        }

        [Test]
        public void Should_retrieve_named_graph_when_getting_property()
        {
            // given
            var propertyMapping = new Mock<IPropertyMapping>();
            propertyMapping.SetupGet(instance => instance.Uri).Returns(Rdf.predicate);
            _mapping.Setup(m => m.PropertyFor("property")).Returns(propertyMapping.Object);

            // when
            Impromptu.InvokeGet(_entityProxy, "property");

            // then
            _graphSelector.Verify(c => c.SelectGraph(_entityId, _mapping.Object, propertyMapping.Object), Times.Once);
        }
    }
}