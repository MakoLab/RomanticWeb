using System;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class ClassMappingTests
    {
        [Test]
        public void Should_not_be_match_for_different_has_Uri()
        {
            // given
            var classMapping=new ClassMapping(new Uri("http://example/#uri1"),false);

            // when
            var result = classMapping.IsMatch(new[] { new Uri("http://example/#uri2") });

            // then
            result.Should().BeFalse();
        }
    }
}