using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
	internal sealed class TermPart<TParentMap>:ITermPart<TParentMap>
        where TParentMap:TermMap
	{
        private readonly TParentMap _propertyMap;

        internal TermPart(TParentMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

        public TParentMap Is(Uri uri)
        {
            _propertyMap.SetUri(uri);
			return _propertyMap;
		}

        public TParentMap Is(string prefix,string predicateName)
        {
            _propertyMap.SetQName(prefix,predicateName);
	        return _propertyMap;
	    }
	}
}