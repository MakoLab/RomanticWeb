using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping RDF predicate to property
    /// </summary>
	public sealed class TermPart<TParentMap> where TParentMap:TermMap
	{
        private readonly TParentMap _propertyMap;

        internal TermPart(TParentMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

        /// <summary>
        /// Maps the term to a fully qualified URI
        /// </summary>
        public TParentMap Is(Uri uri)
        {
            _propertyMap.SetUri(uri);
			return _propertyMap;
		}

        /// <summary>
        /// Maps the term to a QName referenced URI
        /// </summary>
        /// <remarks>The QName must be resolvable from the <see cref="IOntologyProvider"/></remarks>
        public TParentMap Is(string prefix, string predicateName)
        {
            _propertyMap.SetQName(prefix,predicateName);
	        return _propertyMap;
	    }
	}
}