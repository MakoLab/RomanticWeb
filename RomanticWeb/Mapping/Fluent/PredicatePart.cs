using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping RDF predicate to property
    /// </summary>
	public sealed class PredicatePart<TParentMap> where TParentMap:PropertyMap
	{
        private readonly TParentMap _propertyMap;

        internal PredicatePart(TParentMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

        /// <summary>
        /// Maps the property to a fully qualified URI
        /// </summary>
        public TParentMap Is(Uri uri)
		{
			_propertyMap.PredicateUri = uri;
			return _propertyMap;
		}

        /// <summary>
        /// Maps the property to a QName referenced URI
        /// </summary>
        /// <remarks>The QName must be resolvable from the <see cref="IOntologyProvider"/></remarks>
        public TParentMap Is(string prefix, string predicateName)
	    {
	        _propertyMap.NamespacePrefix=prefix;
	        _propertyMap.PredicateName=predicateName;
	        return _propertyMap;
	    }
	}
}