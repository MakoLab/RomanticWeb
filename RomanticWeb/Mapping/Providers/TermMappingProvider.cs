using System;
using Anotar.NLog;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Base class for mapping providers, which return a RDF term mapping
    /// </summary>
    public abstract class TermMappingProviderBase:ITermMappingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermMappingProviderBase"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        protected TermMappingProviderBase(Uri termUri)
        {
            ((ITermMappingProvider)this).GetTerm=provider => termUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermMappingProviderBase"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        protected TermMappingProviderBase(string namespacePrefix,string term)
        {
            ((ITermMappingProvider)this).GetTerm=provider => GetTermUri(provider,namespacePrefix,term);
        }

        /// <summary>
        /// Initializes an empty <see cref="TermMappingProviderBase"/>.
        /// </summary>
        protected TermMappingProviderBase()
        {
        }

        Func<IOntologyProvider,Uri> ITermMappingProvider.GetTerm { get; set; }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="mappingProviderVisitor">The visitor.</param>
        public abstract void Accept(IMappingProviderVisitor mappingProviderVisitor);

        private static Uri GetTermUri(IOntologyProvider ontologyProvider,string namespacePrefix,string termName)
        {
            var resolvedUri=ontologyProvider.ResolveUri(namespacePrefix,termName);

            if (resolvedUri==null)
            {
                var message=string.Format("Cannot resolve QName {0}:{1}",namespacePrefix,termName);
                LogTo.Fatal(message);
                throw new MappingException(message);
            }

            return resolvedUri;
        }
    }
}