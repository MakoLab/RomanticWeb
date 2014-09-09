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
    }
}