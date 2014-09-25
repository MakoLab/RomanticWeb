using System;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Model
{
    [TestFixture]
    public class EntityQuadCollectionTests
    {
        private IEntityQuadCollection _quads;

        [SetUp]
        public void Setup()
        {
            _quads = new EntityQuadCollection2();
        }

        [Test]
        public void Adding_quad_with_blank_subject_should_add_to_root_entity_triples()
        {
            // given
            EntityId root = "urn:root:entity";
            var blankId = new BlankId("test", root);

            // when
            _quads.Add(new EntityQuad(blankId, CreateTriple()));

            // then
            _quads[root].Should().HaveCount(1);
        }

        [Test]
        public void Should_throw_when_blank_id_has_no_root()
        {
            _quads.Invoking(q => q.Add(new EntityQuad(new BlankId("test"), CreateTriple())))
                  .ShouldThrow<ArgumentException>();
        }

        private Triple CreateTriple()
        {
            Node s = Node.ForUri(Rdf.subject);
            Node p = Node.ForUri(Rdf.predicate);
            Node o = Node.ForUri(Rdf.@object);
            return new Triple(s, p, o);
        }
    }
}