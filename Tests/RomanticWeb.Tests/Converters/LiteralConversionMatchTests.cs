using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Converters;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    public class LiteralConversionMatchTests
    {
        [Test]
        public void Negative_matches_should_be_equal()
        {
            // given
            var left = new LiteralConversionMatch();
            var right = new LiteralConversionMatch();

            // when
            var compareTo = left.CompareTo(right);

            // then
            compareTo.Should().Be(0);
            left.Should().Be(right);
        }

        [TestCase(MatchResult.ExactMatch)]
        [TestCase(MatchResult.DontCare)]
        public void Literal_match_should_be_greater_than_no_match(MatchResult literalFormatMatch)
        {
            // given
            var left = new LiteralConversionMatch
            {
                LiteralFormatMatches = literalFormatMatch
            };
            var right = new LiteralConversionMatch();

            // when
            var compareTo = left.CompareTo(right);

            // then
            compareTo.Should().BeGreaterThan(0);
            left.Should().NotBe(right);
        }

        [TestCase(MatchResult.ExactMatch)]
        [TestCase(MatchResult.DontCare)]
        public void Datatype_match_should_be_greater_than_no_match(MatchResult datatypeMatch)
        {
            // given
            var left = new LiteralConversionMatch
            {
                DatatypeMatches = datatypeMatch
            };
            var right = new LiteralConversionMatch();

            // when
            var compareTo = left.CompareTo(right);

            // then
            compareTo.Should().BeGreaterThan(0);
            left.Should().NotBe(right);
        }

        [TestCase(MatchResult.NoMatch)]
        [TestCase(MatchResult.DontCare)]
        public void Exact_literal_match_should_be_greater_than_datatype_match(MatchResult datatypeMatch)
        {
            // given
            var left = new LiteralConversionMatch { LiteralFormatMatches = MatchResult.ExactMatch };
            var right = new LiteralConversionMatch { DatatypeMatches = datatypeMatch };

            // when
            var compareTo = left.CompareTo(right);

            // then
            compareTo.Should().BeGreaterThan(0);
        }
    }
}