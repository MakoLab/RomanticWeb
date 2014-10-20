using System;
using NullGuard;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for dictionary key predicate
    /// </summary>
    public class KeyMappingProvider : TermMappingProviderBase, IPredicateMappingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMappingProvider"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        public KeyMappingProvider(Uri termUri)
            : base(termUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMappingProvider"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        public KeyMappingProvider(string namespacePrefix, string term)
            : base(namespacePrefix, term)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="KeyMappingProvider"/>.
        /// </summary>
        public KeyMappingProvider()
        {
        }

        /// <inheritdoc/>
        public Type ConverterType { [return: AllowNull] get; set; }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
        }
    }
}