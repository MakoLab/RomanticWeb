using System;
using System.Globalization;
using System.Linq;
using ImpromptuInterface;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
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
        private EntityStore _entityStore;

        [SetUp]
        public void Setup()
        {
            _entityStore = new EntityStore(new Mock<RomanticWeb.Updates.IDatasetChangesTracker>().Object);
            _mapping = new Mock<IEntityMapping>(MockBehavior.Strict);
            _context = new Mock<IEntityContext>(MockBehavior.Strict);
            _context.SetupGet(instance => instance.CurrentCulture).Returns(CultureInfo.GetCultureInfo("en"));
            _graphSelector = new Mock<INamedGraphSelector>();

            _context.Setup(c => c.Store).Returns(_entityStore);
            _context.Setup(c => c.InitializeEnitity(It.IsAny<IEntity>()));
            _graphSelector.Setup(g => g.SelectGraph(It.IsAny<EntityId>(), It.IsAny<IEntityMapping>(), It.IsAny<IPropertyMapping>()))
                          .Returns(new Uri("urn:default:graph"));

            var entity = new Entity(_entityId, _context.Object);
            _entityProxy = new EntityProxy(entity, _mapping.Object, new TestTransformerCatalog(), _graphSelector.Object);
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

        [Test]
        public void Should_convert_to_absolute_a_relative_uri_EntityId_property_value()
        {
            // given
            var baseUri = new Uri("http://test.org/");
            var baseUriSelectonPolicy = new Mock<IBaseUriSelectionPolicy>();
            baseUriSelectonPolicy.Setup(instance => instance.SelectBaseUri(It.IsAny<EntityId>())).Returns(baseUri);

            var propertyMapping = new Mock<IPropertyMapping>();
            propertyMapping.SetupGet(instance => instance.Uri).Returns(Rdf.predicate);
            propertyMapping.SetupGet(instance => instance.Converter).Returns(new EntityIdConverter(baseUriSelectonPolicy.Object));
            _mapping.Setup(m => m.PropertyFor("property")).Returns(propertyMapping.Object);

            var value = new Uri("/test", UriKind.Relative);

            // when
            Impromptu.InvokeSet(_entityProxy, "property", new EntityId(value));

            // then
            _entityStore.Quads.Any(item => (item.Object.IsUri) && (item.Object.Uri.AbsoluteUri == new Uri(baseUri, value).AbsoluteUri));
        }
    }
}