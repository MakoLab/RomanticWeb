using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Resourcer;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Vocabularies;
using VDS.RDF;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityStoreTests
    {
        private static readonly Uri GraphUri = new Uri("http://data.magi/people/Tomasz");
        private static readonly EntityId EntityId = new EntityId("http://magi/people/Tomasz");
        private EntityStore _entityStore;

        [SetUp]
        public void Setup()
        {
            _entityStore = new EntityStore();
        }

        [Test]
        public void Should_replace_whole_subgraph_for_blank_node_values()
        {
            // given
            _entityStore.AssertEntity(EntityId, GetGraphWithBlankNodes());
            var property = Node.ForUri(Foaf.knows);

            // when
            _entityStore.ReplacePredicateValues(EntityId, property, () => new Node[0], GraphUri);

            // then
            _entityStore.Quads.Should().HaveCount(0);
        }

        private static IEnumerable<EntityQuad> GetGraphWithBlankNodes()
        {
            var store = new TripleStore();
            store.LoadFromString(Resource.AsString("TestGraphs.BlankNodes.trig"));

            Debug.WriteLine("Loading original graph with {0} triples", store.Graphs[GraphUri].Triples.Count());
            return from triple in store.Graphs[GraphUri].Triples
                   select triple.ToEntityQuad(EntityId);
        }
    }
}