using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping RDF predicate to a mapped element
    /// </summary>
    public interface ITermPart<out TParentMap>
        where TParentMap:TermMap
    {
        /// <summary>
        /// Maps the term to a fully qualified URI
        /// </summary>
        TParentMap Is(Uri uri);

        /// <summary>
        /// Maps the term to a QName referenced URI
        /// </summary>
        /// <remarks>The QName must be resolvable from the <see cref="IOntologyProvider"/></remarks>
        TParentMap Is(string prefix, string predicateName);
    }
}