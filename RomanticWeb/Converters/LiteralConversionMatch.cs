using System;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Literal node conversion matching result
    /// </summary>
    public enum MatchResult
    {
        /// <summary>
        /// The node doesn't match
        /// </summary>
        NoMatch,

        /// <summary>
        /// The matching result is irrelevant for conversion
        /// </summary>
        DontCare,

        /// <summary>
        /// The node does match
        /// </summary>
        ExactMatch
    }

    /// <summary>
    /// Result of matching literals for conversion by <see cref="ILiteralNodeConverter"/>
    /// </summary>
    public struct LiteralConversionMatch:IComparable<LiteralConversionMatch>
    {
        /// <summary>
        /// Gets or sets the value indicating whether the literal node's datatype matched
        /// </summary>
        public MatchResult DatatypeMatches { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the literal node's value format matched
        /// </summary>
        public MatchResult LiteralFormatMatches { get; set; }

#pragma warning disable 1591
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
#pragma warning restore
    }
}