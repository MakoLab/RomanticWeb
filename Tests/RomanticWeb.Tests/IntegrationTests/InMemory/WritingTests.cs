using FluentAssertions;
using System;
using System.Linq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
{
    [TestFixture]
    public class WritingTests:WritingTestsBase
    {
        private TripleStore _store;

        protected TripleStore Store
        {
            get
            {
                if (_store==null)
                {
                    _store=new TripleStore();
                }

                return _store;
            }
        }

        [Test]
        public override void Should_commit_uri_node()
        {
            base.Should_commit_uri_node();
            _store.Triples.Count().Should().Be(2);
        }

        [Test]
        public override void Should_commit_literal_node()
        {
            base.Should_commit_literal_node();
            _store.Triples.Count().Should().Be(3);
        }

        [Test]
        public override void Should_commit_blank_node()
        {
            base.Should_commit_blank_node();
            _store.Triples.Count().Should().Be(4);
        }

        [Test]
        public override void Should_remove_uri_node()
        {
            base.Should_remove_uri_node();
            _store.Triples.Count().Should().Be(0);
        }

        [Test]
        public override void Should_remove_literal_node()
        {
            base.Should_remove_literal_node();
            _store.Triples.Count().Should().Be(2);
        }

        [Test]
        public override void Should_remove_blank_node()
        {
            base.Should_remove_blank_node();
            _store.Triples.Count().Should().Be(2);
        }

        [Test]
        public override void Should_remove_whole_entity_graph()
        {
            base.Should_remove_whole_entity_graph();
            _store.Triples.Count().Should().Be(2);
        }

        [Test]
        public override void Should_reconstruct_entity()
        {
            base.Should_reconstruct_entity();
            _store.Triples.Count().Should().Be(7);
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'",fileName);
            Store.LoadTestFile(fileName);
        }

        protected override IEntitySource CreateEntitySource()
        {
            return new TripleStoreAdapter(Store);
        }

        protected override void ChildTeardown()
        {
            _store=null;
        }
    }
}