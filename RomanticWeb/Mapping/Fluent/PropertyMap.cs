using System;
using System.Reflection;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for properties
    /// </summary>
    public class PropertyMap : INamedGraphSelectingMap
    {
		private readonly PropertyInfo _propertyInfo;

        internal PropertyMap(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

        /// <summary>
        /// Gets a predicate map part
        /// </summary>
		public PredicatePart<PropertyMap> Predicate
		{
			get
			{
				return new PredicatePart<PropertyMap>(this);
			}
		}

        /// <summary>
        /// Gets a named graph mapping part
        /// </summary>
        public NamedGraphPart<PropertyMap> NamedGraph
        {
            get { return new NamedGraphPart<PropertyMap>(this); }
        }

        IGraphSelectionStrategy INamedGraphSelectingMap.GraphSelector { get; set; }

		internal Uri PredicateUri { get; set; }

        internal string NamespacePrefix { get; set; }

        internal string PredicateName { get; set; }

		protected internal virtual bool IsCollection { get { return false; } }

        protected internal virtual StorageStrategyOption StorageStrategy
        {
            get
            {
                return StorageStrategyOption.None;
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        protected PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

        protected internal virtual IPropertyMapping GetMapping(IOntologyProvider ontologies)
        {
            Uri predicateUri = PredicateUri ?? ontologies.ResolveUri(NamespacePrefix,PredicateName);
            return new PropertyMapping(
                PropertyInfo.PropertyType,
                PropertyInfo.Name,
                predicateUri,
                ((INamedGraphSelectingMap)this).GraphSelector);
        }
    }
}