using System.Reflection;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    public class DictionaryMap:PropertyMap
    {
        internal DictionaryMap(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        protected internal override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            return new DictionaryMapping(PropertyInfo.PropertyType,PropertyInfo.Name,GetTermUri(mappingContext.OntologyProvider));
        }
    }
}