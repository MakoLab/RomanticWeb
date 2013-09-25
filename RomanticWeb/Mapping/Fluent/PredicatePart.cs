using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping RDF predicate to property
    /// </summary>
	public sealed class PredicatePart
	{
		private readonly PropertyMap _propertyMap;

        internal PredicatePart(PropertyMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

        /// <summary>
        /// Gets a named graph mapping part
        /// </summary>
		public NamedGraphPart NamedGraph
		{
			get { return new NamedGraphPart(this, _propertyMap); }
		}

        /// <summary>
        /// Maps the property to a fully qualified URI
        /// </summary>
		public PredicatePart Is(Uri uri)
		{
			_propertyMap.PredicateUri = uri;
			return this;
		}

        /// <summary>
        /// Maps the property to a QName referenced URI
        /// </summary>
        /// <remarks>The QName must be resolvable from the <see cref="IOntologyProvider"/></remarks>
	    public PredicatePart Is(string prefix,string predicateName)
	    {
	        _propertyMap.NamespacePrefix=prefix;
	        _propertyMap.PredicateName=predicateName;
	        return this;
	    }
	}
}