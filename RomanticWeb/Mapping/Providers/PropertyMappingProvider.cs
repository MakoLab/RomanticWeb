using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for property predicate
    /// </summary>
    public class PropertyMappingProvider:TermMappingProviderBase,IPropertyMappingProvider
    {
        private readonly PropertyInfo _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingProvider"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        /// <param name="property">The property.</param>
        public PropertyMappingProvider(Uri termUri,PropertyInfo property)
            :base(termUri)
        {
            _property=property;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingProvider"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        /// <param name="property">The property.</param>
        public PropertyMappingProvider(string namespacePrefix,string term,PropertyInfo property)
            :base(namespacePrefix,term)
        {
            _property=property;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public PropertyInfo PropertyInfo
        {
            get
            {
                return _property;
            }
        }

        /// <summary>
        /// Gets the aggregation.
        /// </summary>
        /// <value>
        /// <see cref="Entities.ResultAggregations.Aggregation.SingleOrDefault"/>
        /// </value>
        public virtual Aggregation? Aggregation
        {
            get
            {
                return Entities.ResultAggregations.Aggregation.SingleOrDefault;
            }
        }

        /// <inheritdoc/>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }
    }
}