using System;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public class ClassMappingProvider:TermMappingProviderBase,IClassMappingProvider
    {
        public ClassMappingProvider(Uri uri)
            :base(uri)
        {
        }

        public ClassMappingProvider(string prefix,string term)
            :base(prefix,term)
        {
        }

        public override void Accept(IMappingProviderVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}