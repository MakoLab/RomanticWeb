using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class ObjectAccessorTests
    {
        private readonly Entity _entity = new Entity(new EntityId(Uri));
        private Mock<IRdfNodeConverter> _entityFactory;
        private Ontology _ontology;
        private Mock<IEntityStore> _graph;
        private const string Uri = "urn:test:identity";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _ontology = new Stubs.TestOntologyProvider().Ontologies.First();
        }

        [SetUp]
        public void Setup()
        {
            _graph = new Mock<IEntityStore>(MockBehavior.Strict);
            _entityFactory = new Mock<IRdfNodeConverter>();
        }

        [TearDown]
        public void Teardown()
        {
            _graph.VerifyAll();
            _entityFactory.VerifyAll();
        }

        [Test]
        public void Getting_known_predicate_should_return_objects()
        {
            // given
            _graph = new Mock<IEntityStore>(MockBehavior.Strict);
            _graph.Setup(g => g.GetObjectsForPredicate(_entity.Id, It.IsAny<Uri>())).Returns(new Node[0]);
            dynamic accessor = new OntologyAccessor(_graph.Object, _entity, _ontology, _entityFactory.Object);

            // when
            var givenName = accessor.givenName;

            // then
            _graph.Verify(s => s.GetObjectsForPredicate(_entity.Id, new DatatypeProperty("givenName").InOntology(_ontology).Uri), Times.Once);
        }

        [Test]
        public void Getting_unknown_predicate_should_use_the_property_name()
        {
            // given
            _graph = new Mock<IEntityStore>(MockBehavior.Strict);
            _graph.Setup(g => g.GetObjectsForPredicate(_entity.Id, It.IsAny<Uri>())).Returns(new Node[0]);
            dynamic accessor = new OntologyAccessor(_graph.Object, _entity, _ontology, _entityFactory.Object);

            // when
            var givenName = accessor.fullName;

            // then
            _graph.Verify(s => s.GetObjectsForPredicate(_entity.Id, new Property("fullName").InOntology(_ontology).Uri), Times.Once);
        }
    }
}