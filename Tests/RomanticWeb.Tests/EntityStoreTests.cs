using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;
using VDS.RDF;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityStoreTests
    {
        private static readonly Uri GraphUri = new Uri("http://data.magi/people/Tomasz");
        private static readonly EntityId EntityId = new EntityId("http://magi/people/Tomasz");
        private static readonly Node MetaGraphNode = Node.ForUri(new Uri("http://app.magi/graphs"));
        private EntityStore _entityStore;
        private Mock<IDatasetChangesTracker> _changesTracker;

        [SetUp]
        public void Setup()
        {
            _changesTracker = new Mock<IDatasetChangesTracker>();
            _entityStore = new EntityStore(_changesTracker.Object);
        }

        [Test]
        public void Should_replace_whole_subgraph_for_blank_node_values()
        {
            // given
            LoadEntities("BlankNodes.trig");
            var property = Node.ForUri(Foaf.knows);

            // when
            _entityStore.ReplacePredicateValues(EntityId, property, () => new Node[0], GraphUri);

            // then
            _entityStore.Quads.Should().HaveCount(0);
        }

        [Test]
        public void Replacing_triples_should_track_graph_change()
        {
            // given
            LoadEntities("TriplesWithLiteralSubjects.trig");
            var property = Node.ForUri(Foaf.givenName);
            var newValue = Node.ForLiteral("Tomek");

            // when
            _entityStore.ReplacePredicateValues(EntityId, property, () => new[] { newValue }, GraphUri);

            // then
            _entityStore.Quads.Where(q => q.Graph.Uri == GraphUri).Should().HaveCount(6);
            var expectedAddedQuad = new EntityQuad(EntityId, Node.FromEntityId(EntityId), property, newValue).InGraph(GraphUri);
            var expectedRemovedQuad = new EntityQuad(EntityId, Node.FromEntityId(EntityId), property, Node.ForLiteral("Tomasz")).InGraph(GraphUri);
            _changesTracker.Verify(
                c => c.Add(It.Is<GraphUpdate>(gu => gu.AddedQuads.Contains(expectedAddedQuad) && gu.RemovedQuads.Contains(expectedRemovedQuad))));
        }

        [Test]
        public void Deleting_entity_should_remove_graphs()
        {
            // given
            LoadEntities("TriplesInNamedGraphs.trig");

            // when
            _entityStore.Delete(EntityId);

            // then
            _entityStore.Quads.Should().HaveCount(0);
            _changesTracker.Verify(ct => ct.Add(It.IsAny<GraphDelete>()), Times.Exactly(2));
            _changesTracker.Verify(ct => ct.Add(It.Is<GraphDelete>(gd => gd.Graph == "friendsOf://magi/people/Tomasz")));
            _changesTracker.Verify(ct => ct.Add(It.Is<GraphDelete>(gd => gd.Graph == "personal://magi/people/Tomasz")));
        }

        private void LoadEntities(string fileName)
        {
            var store = new TripleStore();
            store.LoadTestFile(fileName);

            Console.WriteLine("Loading data with {0} triples in {1} graphs", store.Triples.Count(), store.Graphs.Count);

            var data = from metaTriple in store[MetaGraphNode.Uri].GetTriplesWithPredicate(Foaf.primaryTopic)
                       let entityGraph = store[((IUriNode)metaTriple.Subject).Uri]
                       from entityTriple in entityGraph.Triples
                       let entityId = new EntityId(((IUriNode)metaTriple.Object).Uri)
                       let entityQuad = entityTriple.ToEntityQuad(entityId)
                       group entityQuad by entityId into g
                       select g;

            foreach (var entityQuads in data)
            {
                _entityStore.AssertEntity(entityQuads.Key, entityQuads);
            }
        }
    }
}