using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for property predicate
    /// </summary>
    public class PropertyMappingProvider : TermMappingProviderBase, IPropertyMappingProvider
    {
        private readonly PropertyInfo _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingProvider"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        /// <param name="property">The property.</param>
        public PropertyMappingProvider(Uri termUri, PropertyInfo property)
            : base(termUri)
        {
            _property = property;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingProvider"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        /// <param name="property">The property.</param>
        public PropertyMappingProvider(string namespacePrefix, string term, PropertyInfo property)
            : base(namespacePrefix, term)
        {
            _property = property;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get
            {
                return _property;
            }
        }

        /// <inheritdoc/>
        public Type ConverterType { [return: AllowNull] get; set; }

        /// <inheritdoc/>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }

#pragma warning disable 1591
        public override string ToString()
        {
            return string.Format("Property {0}", PropertyInfo.Name);
        }
#pragma warning restore
    }
}