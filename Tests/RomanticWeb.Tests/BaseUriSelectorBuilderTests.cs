using System;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class BaseUriSelectorBuilderTests
    {
        private BaseUriSelectorBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder=new BaseUriSelectorBuilder();
        }

        [Test]
        public void Should_allow_setting_same_base_Uri_for_all_relative_Uris()
        {
            // given
            var baseUri=new Uri("http://some/base/uri/");
            _builder.Default.Is(baseUri);

            // when
            var policy=_builder.Build();

            // then
            policy.Should().BeOfType<ConstantBaseUri>();
            policy.As<ConstantBaseUri>().BaseUri.Should().Be(baseUri);
        }

        [Test,ExpectedException(typeof(ArgumentException))]
        public void Should_throw_if_default_Uri_is_relative()
        {
            // given
            var baseUri=new Uri("some/base/uri/",UriKind.Relative);

            // then
            _builder.Default.Is(baseUri);
        }
    }
}