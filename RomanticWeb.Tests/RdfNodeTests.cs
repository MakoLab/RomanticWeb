using System;
using NUnit.Framework;
using RomanticWeb.Model;
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
            Node uri1 = Node.ForUri(new Uri("http://magi/text#uri1"));
            Node uri2 = Node.ForUri(new Uri("http://magi/text#uri2"));

            // then
            Assert.That(uri1, Is.Not.EqualTo(uri2));
        }
    }
}