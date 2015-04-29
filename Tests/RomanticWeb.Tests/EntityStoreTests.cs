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
        public void Deleting_entity_should_remove_whole_entity()
        {
            // given
            LoadEntities("TriplesInNamedGraphs.trig");

            // when
            _entityStore.Delete(EntityId);

            // then
            _entityStore.Quads.Should().HaveCount(0);
            _changesTracker.Verify(ct => ct.Add(It.IsAny<EntityDelete>()), Times.Exactly(1));
            _changesTracker.Verify(ct => ct.Add(It.Is<EntityDelete>(gd => gd.Entity == EntityId)));
        }

        [Test]
        public void Rollback_should_clear_changes()
        {
            // when
            _entityStore.Rollback();

            // then
            _changesTracker.Verify(c => c.Clear());
        }

        [Test]
        public void Rollback_should_not_discard_loaded_entities()
        {
            // given
            LoadEntities("TriplesWithLiteralSubjects.trig");

            // when
            _entityStore.Rollback();

            // then
            _entityStore.Quads.Where(q => q.Graph.Uri == GraphUri).Should().HaveCount(6);
        }

        [Test]
        public void Rollback_should_revert_changes()
        {
            // given
            LoadEntities("TriplesWithLiteralSubjects.trig");
            var property = Node.ForUri(Foaf.givenName);
            var newValue = Node.ForLiteral("Tomek");
            _entityStore.ReplacePredicateValues(EntityId, property, () => new[] { newValue }, GraphUri);

            // when
            _entityStore.Rollback();

            // then
            _entityStore.Quads.Where(q => q.Graph.Uri == GraphUri).Should().HaveCount(6);
            (from quad in _entityStore.Quads 
             where quad.Predicate == property 
                && quad.Object == newValue 
             select quad)
             .Should().HaveCount(0);
        }

        [Test]
        public void Asserting_entities_from_shared_graphs_should_not_duplicate_statements()
        {
            var graph = new Uri("http://temp.uri/graph");
            var predicateUri = new Uri("http://temp.uri/vocab#predicate");
            var otherEntityId = new EntityId("http://temp.uri/root-entity/other-entity");
            var entityId = new EntityId("http://temp.uri/root-entity");
            var quads = new[] { new EntityQuad(entityId, Node.ForUri(otherEntityId.Uri), Node.ForUri(predicateUri), Node.ForLiteral("test"), Node.ForUri(graph)) };
            _entityStore.AssertEntity(entityId, quads);

            var otherQuads = new[] { new EntityQuad(otherEntityId, Node.ForUri(otherEntityId.Uri), Node.ForUri(predicateUri), Node.ForLiteral("test"), Node.ForUri(graph)) };
            _entityStore.AssertEntity(otherEntityId, otherQuads);

            var statements = _entityStore.GetObjectsForPredicate(otherEntityId, predicateUri, graph);

            statements.Should().HaveCount(1);
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