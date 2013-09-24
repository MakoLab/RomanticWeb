using System;
using NUnit.Framework;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class RdfNodeTests
    {
        [Test]
        public void Hash_URIs_should_not_be_equal()
        {
            // given
            RdfNode uri1 = RdfNode.ForUri(new Uri("http://magi/text#uri1"));
            RdfNode uri2 = RdfNode.ForUri(new Uri("http://magi/text#uri2"));

            // then
            Assert.That(uri1, Is.Not.EqualTo(uri2));
        }
    }
}