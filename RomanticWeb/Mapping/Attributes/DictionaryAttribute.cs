using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    public sealed class DictionaryAttribute:PropertyAttribute
    {
        public DictionaryAttribute(string prefix,string term)
            :base(prefix,term)
        {
        }

        public DictionaryAttribute(string termUri)
            :base(termUri)
        {
        }

        internal override IPropertyMappingProvider Accept(IMappingAttributesVisitor visitor,PropertyInfo property)
        {
            var keyAttribute=property.GetCustomAttributes<KeyAttribute>().SingleOrDefault();
            var valueAttribute=property.GetCustomAttributes<ValueAttribute>().SingleOrDefault();
            return visitor.Visit(property,this,visitor.Visit(keyAttribute),visitor.Visit(valueAttribute));
        }
    }
}