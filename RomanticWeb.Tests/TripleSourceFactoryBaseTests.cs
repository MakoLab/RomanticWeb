using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class TripleSourceFactoryBaseTests
    {
        private TripleSourceFactoryBaseTestable _factory;
        private Mock<IGraphSelectionStrategy> _graphSelector;

        [SetUp]
        public void Setup()
        {
            _factory = new TripleSourceFactoryBaseTestable();
            _graphSelector = new Mock<IGraphSelectionStrategy>(MockBehavior.Strict);
        }

        [TearDown]
        public void Teardown()
        {
            _graphSelector.VerifyAll();
        }

        [Test]
        public void Should_create_default_graph_source_for_property_by_default()
        {
            // given
            var mapping = new Mock<IPropertyMapping>();

            // when
            _factory.CreateTripleSourceForProperty(new UriId("urn:some:identifier"), mapping.Object);

            // then
            Assert.That(_factory.TimesDefaultGraphRequested, Is.EqualTo(1));
            Assert.That(_factory.TimesUnionGraphRequested, Is.EqualTo(0));
            Assert.That(_factory.NamedGraphRequests, Is.Empty);
        }

        [Test]
        public void Should_create_named_graph_source_if_defined_in_property_mapping()
        {
            // given
            var mapping = new Mock<IPropertyMapping>();
            var nameGraphUri = new Uri("urn:some:graph");
            _graphSelector.Setup(gs => gs.SelectGraph(It.IsAny<EntityId>())).Returns(new Uri("urn:some:graph"));
            mapping.Setup(p => p.GraphSelector).Returns(_graphSelector.Object);

            // when
            _factory.CreateTripleSourceForProperty(new UriId("urn:some:identifier"), mapping.Object);

            // then
            Assert.That(_factory.TimesDefaultGraphRequested, Is.EqualTo(0));
            Assert.That(_factory.TimesUnionGraphRequested, Is.EqualTo(0));
            Assert.That(_factory.NamedGraphRequests, Has.Count.EqualTo(1));
            Assert.That(_factory.NamedGraphRequests[nameGraphUri], Is.EqualTo(1));
        }

        [Test]
        public void Should_create_union_graph_source_if_defined_in_property_mapping()
        {
            // given
            var mapping = new Mock<IPropertyMapping>();
            mapping.Setup(p => p.UsesUnionGraph).Returns(true);

            // when
            _factory.CreateTripleSourceForProperty(new UriId("urn:some:identifier"), mapping.Object);

            // then
            Assert.That(_factory.TimesDefaultGraphRequested, Is.EqualTo(0));
            Assert.That(_factory.TimesUnionGraphRequested, Is.EqualTo(1));
            Assert.That(_factory.NamedGraphRequests, Is.Empty);
        }

        class TripleSourceFactoryBaseTestable : TripleSourceFactoryBase
        {
            public int TimesDefaultGraphRequested { get; private set; }
            public int TimesUnionGraphRequested { get; private set; }
            public Dictionary<Uri, int> NamedGraphRequests { get; private set; }

            public TripleSourceFactoryBaseTestable()
            {
                NamedGraphRequests = new Dictionary<Uri, int>();
            }

            protected override ITripleSource CreateSourceForDefaultGraph()
            {
                TimesDefaultGraphRequested++;
                return new Mock<ITripleSource>().Object;
            }

            protected override ITripleSource CreateSourceForNamedGraph(Uri namedGraph)
            {
                if (!NamedGraphRequests.ContainsKey(namedGraph))
                {
                    NamedGraphRequests[namedGraph] = 0;
                }
                NamedGraphRequests[namedGraph]++;
                return new Mock<ITripleSource>().Object;
            }

            protected override ITripleSource CreateSourceForUnionGraph()
            {
                TimesUnionGraphRequested++;
                return new Mock<ITripleSource>().Object;
            }
        }
    }
}