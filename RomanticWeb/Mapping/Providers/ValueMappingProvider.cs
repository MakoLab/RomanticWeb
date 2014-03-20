using System;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public class ValueMappingProvider : TermMappingProviderBase
    {
        public ValueMappingProvider(Uri termUri)
            :base(termUri)
        {
        }

        public ValueMappingProvider(string namespacePrefix,string termName)
            :base(namespacePrefix,termName)
        {
        }

        public ValueMappingProvider()
        {
        }

        public override void Accept(IMappingProviderVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}