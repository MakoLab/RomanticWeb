using System;
using System.Collections;
using System.Collections.Generic;
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
    public class DatasetChangesOptimizerTests
    {
        private const string GraphId = "urn:some:graph";
        private static readonly EntityId EntityId = "urn:test:id";
        private static readonly EntityQuad AQuad = new EntityQuad(EntityId, Node.ForUri(Rdf.subject), Node.ForUri(Rdf.predicate), Node.ForLiteral("string"));
        private DatasetChangesOptimizer _optimizer;
        private TestDatasetChanges _changes;

        [SetUp]
        public void Setup()
        {
            _changes = new TestDatasetChanges();
            _optimizer = new DatasetChangesOptimizer();
        }

        [Test]
        public void Should_ignore_changes_followed_by_graph_delete()
        {
            // given
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));
            _changes.Add(new GraphDelete(EntityId, GraphId));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));
            DatasetChange delete = new GraphDelete(EntityId, GraphId);
            _changes.Add(delete);

            // when
            var optimized = _optimizer.Optimize(_changes).ToList();

            // then
            optimized.Should().HaveCount(1);
            optimized.Single().Should().BeOfType<GraphDelete>();
            optimized.Single().Should().BeSameAs(delete);
        }

        [Test]
        public void Should_combine_multiple_changes_to_one_graph()
        {
            // given
            _changes.Add(new GraphUpdate(EntityId, GraphId, new[] { AQuad }, new EntityQuad[0]));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new[] { AQuad }));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));
            _changes.Add(new GraphUpdate(EntityId, GraphId, new EntityQuad[0], new EntityQuad[0]));

            // when
            var optimized = _optimizer.Optimize(_changes).ToList();

            // then
            optimized.Should().HaveCount(1);
            optimized.Single().Should().BeOfType<GraphUpdate>();
        }

        public class TestDatasetChanges : IDatasetChanges
        {
            private readonly IList<DatasetChange> _changes = new List<DatasetChange>(); 

            public bool HasChanges
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IEnumerable<DatasetChange> this[EntityId graphUri]
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IEnumerator<KeyValuePair<EntityId, IEnumerable<DatasetChange>>> GetEnumerator()
            {
                yield return new KeyValuePair<EntityId, IEnumerable<DatasetChange>>(GraphId, _changes);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            internal void Add(DatasetChange datasetChange)
            {
                _changes.Add(datasetChange);
            }
        }
    }
}