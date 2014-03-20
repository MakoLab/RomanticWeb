using System;
using System.Reflection;

namespace RomanticWeb.Mapping.Providers
{
    public class DictionaryMappingProvider:PropertyMappingProvider,IDictionaryMappingProvider
    {
        private readonly ITermMappingProvider _key;
        private readonly ITermMappingProvider _value;

        public DictionaryMappingProvider(Uri uri,ITermMappingProvider key,ITermMappingProvider value,PropertyInfo property)
            :base(uri,property)
        {
            _key=key;
            _value=value;
        }

        public DictionaryMappingProvider(string prefix,string term,ITermMappingProvider key,ITermMappingProvider value,PropertyInfo property)
            : base(prefix,term,property)
        {
            _key=key;
            _value=value;
        }

        public ITermMappingProvider Key
        {
            get
            {
                return _key;
            }
        }

        public ITermMappingProvider Value
        {
            get
            {
                return _value;
            }
        }

        public override void Accept(Visitors.IMappingProviderVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}