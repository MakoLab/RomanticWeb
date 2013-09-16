using System;
using System.Reflection;

namespace RomanticWeb.Mapping
{
    public class PropertyMap
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyMap(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public PredicatePart Predicate
        {
            get
            {
                return new PredicatePart(this);
            }
        }

        internal Uri PredicateUri { get; set; }

        internal IGraphSelectionStrategy GraphSelector { get; set; }

        protected internal virtual bool IsCollection { get { return false; } }

        public IPropertyMapping GetMapping()
        {
            return new PropertyMapping(_propertyInfo.Name, PredicateUri, GraphSelector);
        }
    }
}