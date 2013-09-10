using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class ObjectAccessorTests
    {
        private readonly Entity _entity = new Entity(new UriId(Uri));
        private Mock<IEntityFactory> _entityFactory;
        private Ontology _ontology;
        private Mock<ITriplesSource> _graph;
        private const string Uri = "urn:test:identity";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _ontology = new Stubs.StaticOntologyProvider().Ontologies.First();
        }

        [SetUp]
        public void Setup()
        {
            _graph = new Mock<ITriplesSource>(MockBehavior.Strict);
            _entityFactory = new Mock<IEntityFactory>();
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
            _graph = new Mock<ITriplesSource>(MockBehavior.Strict);
            _graph.Setup(g => g.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Property>())).Returns(new RdfNode[0]);
            dynamic accessor = new OntologyAccessor(_graph.Object, _entity, _ontology, _entityFactory.Object);

            // when
            var givenName = accessor.givenName;

            // then
            _graph.Verify(s => s.GetObjectsForPredicate(_entity.Id,
                                                        new DatatypeProperty("givenName").InOntology(_ontology)),
                                                        Times.Once);
        }

        [Test]
        public void Getting_unknown_predicate_should_use_the_property_name()
        {
            // given
            _graph = new Mock<ITriplesSource>(MockBehavior.Strict);
            _graph.Setup(g => g.GetObjectsForPredicate(It.IsAny<EntityId>(), It.IsAny<Property>())).Returns(new RdfNode[0]);
            dynamic accessor = new OntologyAccessor(_graph.Object, _entity, _ontology, _entityFactory.Object);

            // when
            var givenName = accessor.fullName;

            // then
            _graph.Verify(s => s.GetObjectsForPredicate(_entity.Id,
                                                        new Property("fullName").InOntology(_ontology)),
                                                        Times.Once);
        }
    }
}