using System;
using NUnit.Framework;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class TripleTests
    {
        private readonly Node[] NonUriNodes = new[]
                                                {
                                                    Node.ForBlank("some_blank",null),
                                                    Node.ForLiteral("literal",null,null)
                                                };

        private Node _validSubject;
        private Node _validObject;
        private Node _validPredicate;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _validSubject = Node.ForUri(new Uri("http://magi/test/subject"));
            _validObject = Node.ForUri(new Uri("http://magi/test/object"));
            _validPredicate = Node.ForUri(new Uri("http://magi/test/predicate"));
        }

        [Test]
        public void Created_triple_should_expose_its_terms()
        {
            // when
            var triple=new Triple(_validSubject,_validPredicate,_validObject);

            // then
            Assert.That(triple.Subject, Is.SameAs(_validSubject));
            Assert.That(triple.Predicate, Is.SameAs(_validPredicate));
            Assert.That(triple.Object, Is.SameAs(_validObject));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestCaseSource("NonUriNodes")]
        public void Predicate_must_be_a_URI_node(Node predicate)
        {
            new Triple(_validSubject, predicate, _validObject);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Subject_must_not_be_a_literal_node()
        {
            // given
            var subject=Node.ForLiteral("literal",null,null);

            // then
            new Triple(subject, _validPredicate, _validObject);
        }
    }
}