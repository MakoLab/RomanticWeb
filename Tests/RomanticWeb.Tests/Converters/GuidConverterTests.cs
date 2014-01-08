using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    public class GuidConverterTests
    {
        private GuidConverter _converter;

        [SetUp]
        public void Setup()
        {
            _converter=new GuidConverter();
        }

        [TearDown]
        public void Teardown()
        {
        }

        [TestCase("6cbb1450-c74d-47e8-a1e0-50f1db6cfd6f")]
        [TestCase("6cbb1450c74d47e8a1e050f1db6cfd6f")]
        public void Should_match_conversion_of_untyped_GUID_literal(string guid)
        {
            // given

            // when
            var match=_converter.CanConvert(Node.ForLiteral(guid));

            // then
            match.LiteralFormatMatches.Should().Be(MatchResult.ExactMatch);
            match.DatatypeMatches.Should().Be(MatchResult.DontCare);
        }
    }
}