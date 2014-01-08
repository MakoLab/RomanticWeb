using System;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class ConstantBaseUriSelectionPolicyTests
    {
        [Test]
        public void Should_throw_when_constructing_with_relative_Uri()
        {
            // given
            var baseUri=new Uri("some/base/uri/",UriKind.Relative);

            // then
            Assert.Throws<ArgumentException>(() => new ConstantBaseUri(baseUri));
        }

        [Test]
        public void Should_return_the_given_URI()
        {
            // given
            var baseUri = new Uri("http://some/base/uri/");
            var selector = new ConstantBaseUri(baseUri);

            // when
            var selectedBaseUri=selector.SelectBaseUri(new EntityId("some/identifier"));

            // then
            selectedBaseUri.Should().Be(baseUri);
        }
    }
}