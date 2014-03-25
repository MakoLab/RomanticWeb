using System;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for dictionary value property
    /// </summary>
    public class ValueMappingProvider:TermMappingProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueMappingProvider"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        public ValueMappingProvider(Uri termUri)
            :base(termUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueMappingProvider"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        public ValueMappingProvider(string namespacePrefix,string term)
            :base(namespacePrefix,term)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="ValueMappingProvider"/>.
        /// </summary>
        public ValueMappingProvider()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
        }
    }
}