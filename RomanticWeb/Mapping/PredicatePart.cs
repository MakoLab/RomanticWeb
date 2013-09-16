using System;

namespace RomanticWeb.Mapping
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
            get { return new NamedGraphPart(this, _propertyMap); }
        }

        public PredicatePart Is(Uri uri)
        {
            _propertyMap.PredicateUri = uri;
            return this;
        }
    }
}