using System;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public class KeyMappingProvider : TermMappingProviderBase
    {
        public KeyMappingProvider(Uri termUri)
            :base(termUri)
        {
        }

        public KeyMappingProvider(string namespacePrefix,string termName)
            :base(namespacePrefix,termName)
        {
        }

        public KeyMappingProvider()
        {
        }

        public override void Accept(IMappingProviderVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}