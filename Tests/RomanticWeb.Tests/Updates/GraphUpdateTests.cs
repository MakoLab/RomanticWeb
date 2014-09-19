using System.Collections;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Updates
{
    [TestFixture]
    public class GraphUpdateTests
    {
        private static readonly EntityId GraphId = "urn:some:graph";
        private static readonly EntityId EntityId = "urn:test:id";
        private static readonly EntityQuad AQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("A"));
        private static readonly EntityQuad BQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("B"));
        private static readonly EntityQuad CQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("C"));
        
        [Test]
        public void Should_cancel_out_quads_both_delete_and_added()
        {
            // given
            var removedQuads = new[] { AQuad };
            var addedQuads = new[] { AQuad, BQuad };

            // when
            var update = new GraphUpdate(EntityId, GraphId, removedQuads, addedQuads);

            // then
            update.AddedQuads.Single().Should().Be(BQuad);
            update.RemovedQuads.Should().BeEmpty();
        }

        [Test]
        public void Should_remove_duplicate_removed_quads()
        {
            // given
            var removedQuads = new[] { AQuad, AQuad, AQuad, AQuad };

            // when
            var update = new GraphUpdate(EntityId, GraphId, removedQuads, new EntityQuad[0]);

            // then
            update.AddedQuads.Should().BeEmpty();
            update.RemovedQuads.Should().HaveCount(1);
        }

        [Test]
        public void Should_remove_duplicate_added_quads()
        {
            // given
            var addedQuads = new[] { AQuad, AQuad, AQuad, AQuad };

            // when
            var update = new GraphUpdate(EntityId, GraphId, new EntityQuad[0], addedQuads);

            // then
            update.AddedQuads.Should().HaveCount(1);
            update.RemovedQuads.Should().BeEmpty();
        }

        [Test]
        public void Should_be_mergeable_with_another()
        {
            // given
            var removedQuads = new[] { AQuad };
            var addedQuads = new[] { AQuad, BQuad };

            // when
            var update = new GraphUpdate(EntityId, GraphId, removedQuads, addedQuads);
            var other = new GraphUpdate(EntityId, GraphId, removedQuads, addedQuads);

            // then
            update.CanMergeWith(other).Should().BeTrue();
        }

        [Test]
        public void Merging_should_combine_two_updates()
        {
            // given
            var removedQuads = new[] { AQuad };
            var addedQuads = new[] { CQuad, BQuad };
            var update = new GraphUpdate(EntityId, GraphId, removedQuads, new EntityQuad[0]);
            var other = new GraphUpdate(EntityId, GraphId, removedQuads, addedQuads);

            // when
            var merged = (GraphUpdate)update.MergeWith(other);

            // then
            merged.AddedQuads.Should().Contain((IEnumerable)addedQuads);
            merged.RemovedQuads.Should().Contain((IEnumerable)removedQuads);
        }
    }
}