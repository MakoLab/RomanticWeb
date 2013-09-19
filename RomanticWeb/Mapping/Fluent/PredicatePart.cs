using System;

namespace RomanticWeb.Mapping.Fluent
{
	public class PredicatePart
	{
		private readonly PropertyMap _propertyMap;

		public PredicatePart(PropertyMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

		public NamedGraphPart NamedGraph
		{
			get { return new NamedGraphPart(this, this._propertyMap); }
		}

		public PredicatePart Is(Uri uri)
		{
			_propertyMap.PredicateUri = uri;
			return this;
		}

	    public PredicatePart Is(string prefix,string predicateName)
	    {
	        _propertyMap.NamespacePrefix=prefix;
	        _propertyMap.PredicateName=predicateName;
	        return this;
	    }
	}
}