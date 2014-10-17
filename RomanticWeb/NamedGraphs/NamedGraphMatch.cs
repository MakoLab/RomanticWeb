using System;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>Enumerates possible graph match results.</summary>
    public enum MatchResult
    {
        /// <summary>Indiceates that there was no match.</summary>
        NoMatch,

        /// <summary>Indicates that there was an exact match.</summary>
        ExactMatch,

        /// <summary>Indicates that there was a partial match.</summary>
        PartialMatch,

        /// <summary>Indicates that the implementour don't care about given match.</summary>
        DontCare
    }

    /// <summary>Carries information on given graph match.</summary>
    public struct NamedGraphMatch
    {
        /// <summary>Creates an instance of the <see cref="NamedGraphMatch"/>.</summary>
        /// <param name="namedGraph"><see cref="Uri"/> of the named graph.</param>
        /// <param name="idMatch">Match for entity identifiers.</param>
        /// <param name="predicateMatch">Match for predicates.</param>
        /// <param name="typeMatch">Match for types.</param>
        public NamedGraphMatch(Uri namedGraph, MatchResult idMatch, MatchResult predicateMatch, MatchResult typeMatch)
            : this()
        {
            NamedGraph = namedGraph;
            IdMatch = idMatch;
            PredicateMatch = predicateMatch;
            TypeMatch = typeMatch;
        }

        /// <summary>Gets an <see cref="Uri"/> of the named graph.</summary>
        public Uri NamedGraph { get; private set; }

        /// <summary>Gets a match for entity identifiers.</summary>
        public MatchResult IdMatch { get; private set; }

        /// <summary>Gets a match for predicates.</summary>
        public MatchResult PredicateMatch { get; private set; }

        /// <summary>Gets a match for types.</summary>
        public MatchResult TypeMatch { get; private set; }
    }
}