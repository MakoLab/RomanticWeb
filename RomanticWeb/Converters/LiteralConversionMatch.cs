using System;

namespace RomanticWeb.Converters
{
    public enum MatchResult
    {
        NoMatch,
        DontCare,
        ExactMatch
    }

    public struct LiteralConversionMatch:IComparable<LiteralConversionMatch>
    {
        public MatchResult DatatypeMatches { get; set; }

        public MatchResult LiteralFormatMatches { get; set; }

        public int CompareTo(LiteralConversionMatch other)
        {
            if (LiteralFormatMatches!=other.LiteralFormatMatches)
            {
                return LiteralFormatMatches.CompareTo(other.LiteralFormatMatches);
            }

            if (DatatypeMatches!=other.DatatypeMatches)
            {
                return DatatypeMatches.CompareTo(other.DatatypeMatches);
            }

            return 0;
        }
    }
}