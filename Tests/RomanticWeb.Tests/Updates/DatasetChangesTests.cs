using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Updates;

namespace RomanticWeb.Tests.Updates
{
    [TestFixture]
    public class DatasetChangesTests
    {
        private static readonly EntityId GraphA = "urn:graph:A";
        private static readonly EntityId Entity = "urn:test:entity";

        private DatasetChanges _changes;

        [SetUp]
        public void Setup()
        {
            _changes = new DatasetChanges();
        }

        [Test]
        public void Should_combine_graph_changes_to_same_graph()
        {
            // when
            _changes.Add(new GraphUpdate(Entity, GraphA, RandomQuads(2).ToArray(), RandomQuads(3).ToArray()));
            _changes.Add(new GraphUpdate(Entity, GraphA, RandomQuads(1).ToArray(), RandomQuads(5).ToArray()));

            // then
            _changes.Should().HaveCount(1);
            _changes[GraphA].Should().HaveCount(1);
            _changes.Single().Should().Match((GraphUpdate g) => g.AddedQuads.Count() == 8 && g.RemovedQuads.Count() == 3);
        }

        [Test]
        public void Should_ignore_empty_changes()
        {
            // given
            var change = new GraphUpdate(Entity, GraphA, new EntityQuad[0], new EntityQuad[0]);

            // when
            _changes.Add(change);

            // then
            _changes.HasChanges.Should().BeFalse();
            _changes.Should().HaveCount(0);
        }

        private IEnumerable<EntityQuad> RandomQuads(int count)
        {
            var randomNode = new Func<Node>(() => Node.ForUri(new Uri("node://" + Guid.NewGuid().ToString("N"))));

            for (var i = 0; i < count; i++)
            {
                yield return new EntityQuad(Entity, randomNode(), randomNode(), randomNode());
            }
        }
    }
}