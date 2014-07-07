using System;

namespace RomanticWeb.NamedGraphs
{
    public enum MatchResult
    {
        NoMatch,

        ExactMatch,

        PartialMatch,

        DontCare
    }

    public struct NamedGraphMatch
    {
        public NamedGraphMatch(Uri namedGraph, MatchResult idMatch, MatchResult predicateMatch, MatchResult typeMatch)
            : this()
        {
            NamedGraph = namedGraph;
            IdMatch = idMatch;
            PredicateMatch = predicateMatch;
            TypeMatch = typeMatch;
        }

        public Uri NamedGraph { get; private set; }

        public MatchResult IdMatch { get; private set; }

        public MatchResult PredicateMatch { get; private set; }

        public MatchResult TypeMatch { get; private set; }
    }
}