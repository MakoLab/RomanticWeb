using System;
using Anotar.NLog;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    public abstract class TermMappingProviderBase:ITermMappingProvider
    {
        private Func<IOntologyProvider,Uri> _getTerm;

        protected TermMappingProviderBase(Uri termUri)
        {
            _getTerm=provider => termUri;
        }

        protected TermMappingProviderBase(string namespacePrefix,string termName)
        {
            _getTerm=provider => GetTermUri(provider,namespacePrefix,termName);
        }

        protected TermMappingProviderBase()
        {
        }

        Func<IOntologyProvider,Uri> ITermMappingProvider.GetTerm
        {
            get
            {
                return _getTerm;
            }

            set
            {
                _getTerm=value;
            }
        }

        public abstract void Accept(IMappingProviderVisitor visitor);

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