using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class OntologyAccessorTests
    {
        private const string Uri = "urn:test:identity";
        private Entity _entity;
        private Mock<INodeConverter> _nodeProcessor;
        private Ontology _ontology;
        private Mock<IEntityStore> _store;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _ontology = new TestOntologyProvider().Ontologies.First();
        }

        [SetUp]
        public void Setup()
        {
            var _context = new Mock<IEntityContext>();
            _store = new Mock<IEntityStore>(MockBehavior.Strict);
            _context.Setup(c => c.Store).Returns(_store.Object);
            _entity = new Entity(new EntityId(Uri), _context.Object);
            _entity.MarkAsInitialized();
            _nodeProcessor = new Mock<INodeConverter>();
        }

        [TearDown]
        public void Teardown()
        {
            _store.VerifyAll();
            _nodeProcessor.VerifyAll();
        }

        [Test]
        public void Getting_known_predicate_should_return_objects()
        {
            // given
            IServiceContainer container = new ServiceContainer();
            _store.Setup(g => g.GetObjectsForPredicate(_entity.Id, It.IsAny<Uri>(), It.IsAny<Uri>())).Returns(new Node[0]);
            dynamic accessor = new OntologyAccessor(_entity, _ontology, new FallbackNodeConverter(new ConverterCatalog(null)), new TestTransformerCatalog());

            // when
            var givenName = accessor.givenName;

            // then
            _store.Verify(s => s.GetObjectsForPredicate(_entity.Id, new DatatypeProperty("givenName").InOntology(_ontology).Uri, null), Times.Once);
        }

        [Test]
        public void Getting_unknown_predicate_should_use_the_property_name()
        {
            // given
            IServiceContainer container = new ServiceContainer();
            _store.Setup(g => g.GetObjectsForPredicate(_entity.Id, It.IsAny<Uri>(), It.IsAny<Uri>())).Returns(new Node[0]);
            dynamic accessor = new OntologyAccessor(_entity, _ontology, new FallbackNodeConverter(new ConverterCatalog(null)), new TestTransformerCatalog());

            // when
            var givenName = accessor.fullName;

            // then
            _store.Verify(s => s.GetObjectsForPredicate(_entity.Id, new Property("fullName").InOntology(_ontology).Uri, null), Times.Once);
        }
    }
}