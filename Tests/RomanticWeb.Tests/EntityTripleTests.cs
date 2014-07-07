using System;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityTripleTests
    {
        private readonly Node[] _nonUriNodes = new[] { Node.ForLiteral("literal") };

        private Node _validSubject;
        private Node _validObject;
        private Node _validPredicate;
        private Node _graph;
        private EntityId _entityId;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _validSubject = Node.ForUri(new Uri("http://magi/test/subject"));
            _validObject = Node.ForUri(new Uri("http://magi/test/object"));
            _validPredicate = Node.ForUri(new Uri("http://magi/test/predicate"));
            _graph = Node.ForUri(new Uri("urn:some:graph"));
            _entityId = new EntityId(new Uri("http://magi/test/subject"));
        }

        [Test]
        public void Created_triple_should_expose_its_terms()
        {
            // when
            var triple = new EntityQuad(_entityId, _validSubject, _validPredicate, _validObject, _graph);

            // then
            Assert.That(triple.Subject, Is.SameAs(_validSubject));
            Assert.That(triple.Predicate, Is.SameAs(_validPredicate));
            Assert.That(triple.Object, Is.SameAs(_validObject));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestCaseSource("_nonUriNodes")]
        public void Predicate_must_be_either_an_URI_node_or_Blank_node(Node predicate)
        {
            new EntityQuad(_entityId, _validSubject, predicate, _validObject, _graph);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestCaseSource("_nonUriNodes")]
        public void Graph_must_be_either_an_URI_node_or_Blank_node(Node graph)
        {
            new EntityQuad(_entityId, _validSubject, _validPredicate, _validObject, graph);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Subject_must_not_be_a_literal_node()
        {
            // given
            var subject = Node.ForLiteral("literal");

            // then
            new EntityQuad(_entityId, subject, _validPredicate, _validObject, _graph);
        }

        [Test]
        public void Can_be_ceated_without_graph()
        {
            // when 
            var triple = new EntityQuad(_entityId, _validSubject, _validPredicate, _validObject);

            // then
            Assert.That(triple.Graph, Is.Null);
        }

        [Test]
        public void Triples_should_be_equal_when_id_and_nodes_are_equal()
        {
            // given
            IComparable triple = new EntityQuad(
                new EntityId(new Uri("urn:some:entity")),
                Node.ForUri(new Uri("urn:some:subject")),
                Node.ForUri(new Uri("urn:some:predicate")),
                Node.ForLiteral("10"));
            var otherTriple = new EntityQuad(
                new EntityId(new Uri("urn:some:entity")),
                Node.ForUri(new Uri("urn:some:subject")),
                Node.ForUri(new Uri("urn:some:predicate")),
                Node.ForLiteral("10"));

            // then
            Assert.That(triple.CompareTo(otherTriple), Is.EqualTo(0));
        }
    }
}