using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    public class DictionaryAttribute:PropertyAttribute
    {
        public DictionaryAttribute(string prefix,string term)
            :base(prefix,term)
        {
        }

        public DictionaryAttribute(string termUri)
            :base(termUri)
        {
        }

        protected override IPropertyMapping GetMappingInternal(Type propertyType, string propertyName, Uri uri, MappingContext mappingContext)
        {
            return new DictionaryMapping(propertyType,propertyName,uri);
        }
    }
}