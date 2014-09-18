using System.Collections;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Updates
{
    [TestFixture]
    public class GraphReconstructTests
    {
        private static readonly EntityId GraphId = "urn:some:graph";
        private static readonly EntityId EntityId = "urn:test:id";
        private static readonly EntityQuad AQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("A"));
        private static readonly EntityQuad BQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("B"));
        private static readonly EntityQuad CQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("C"));

        [Test]
        public void Merging_should_combine_recreate_graph_with_update()
        {
            // given
            var newGraph = new[] { AQuad, CQuad };
            var removedQuads = new[] { AQuad };
            var addedQuads = new[] { CQuad, BQuad };
            var update = new GraphReconstruct(EntityId, GraphId, newGraph);
            var other = new GraphUpdate(EntityId, GraphId, removedQuads, addedQuads);

            // when
            var merged = (GraphReconstruct)update.MergeWith(other);

            // then
            merged.AddedQuads.Should().Contain((IEnumerable)addedQuads);
        }

        [Test]
        public void Merging_should_combine_recreate_graph_with_another_by_discarding_the_former()
        {
            // given
            var graphFirst = new[] { AQuad };
            var graphSecond = new[] { CQuad, BQuad };
            var update = new GraphReconstruct(EntityId, GraphId, graphFirst);
            var other = new GraphReconstruct(EntityId, GraphId, graphSecond);

            // when
            var merged = (GraphReconstruct)update.MergeWith(other);

            // then
            merged.Should().Be(other);
        }

        [Test]
        public void Should_be_mergable_with_update()
        {
            // when
            var reconstruct = new GraphReconstruct(EntityId, GraphId, new EntityQuad[0]);
            var update = new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]);

            // then
            reconstruct.CanMergeWith(update).Should().BeTrue();
        }

        [Test]
        public void Should_be_mergable_with_other()
        {
            // when
            var reconstruct = new GraphReconstruct(EntityId, GraphId, new EntityQuad[0]);
            var other = new GraphReconstruct(EntityId, GraphId, new EntityQuad[0]);

            // then
            reconstruct.CanMergeWith(other).Should().BeTrue();
        }
    }
}