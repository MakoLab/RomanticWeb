using System;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider for RDF class
    /// </summary>
    public class ClassMappingProvider:TermMappingProviderBase,IClassMappingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMappingProvider"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public ClassMappingProvider(Uri uri)
            :base(uri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassMappingProvider"/> class.
        /// </summary>
        /// <param name="prefix">The QName prefix.</param>
        /// <param name="term">The QName term.</param>
        public ClassMappingProvider(string prefix,string term)
            :base(prefix,term)
        {
        }

        /// <inheritdoc/>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }
    }
}