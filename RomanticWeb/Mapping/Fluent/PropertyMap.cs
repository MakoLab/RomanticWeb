using System;
using System.Reflection;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    public class PropertyMap
    {
		private readonly PropertyInfo _propertyInfo;

		public PropertyMap(PropertyInfo propertyInfo)
		{
			this._propertyInfo = propertyInfo;
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

        internal string NamespacePrefix { get; set; }

        internal string PredicateName { get; set; }

		protected internal virtual bool IsCollection { get { return false; } }

        internal IPropertyMapping GetMapping(IOntologyProvider ontologies)
        {
            Uri predicateUri = PredicateUri ?? ontologies.ResolveUri(NamespacePrefix,PredicateName);
            return new PropertyMapping(this._propertyInfo.Name, predicateUri, this.GraphSelector, this.IsCollection);
        }
    }
}