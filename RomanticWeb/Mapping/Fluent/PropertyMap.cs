using System;
using System.Reflection;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for properties
    /// </summary>
    public class PropertyMap
    {
		private readonly PropertyInfo _propertyInfo;

        internal PropertyMap(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
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
            return new PropertyMapping(_propertyInfo.Name, predicateUri, GraphSelector, IsCollection);
        }
    }
}