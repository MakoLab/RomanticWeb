using System.Reflection;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for properties.</summary>
    public sealed class PropertyMap:PropertyMapBase<PropertyMap>
    {
        public PropertyMap(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        public override ITermPart<PropertyMap> Term
        {
            get
            {
                return new TermPart<PropertyMap>(this);
            }
        }
        
        /// <summary>Creates a mapping from this definition.</summary>
        public override IPropertyMapping GetMapping(MappingContext mappingContext)
        {
            return new PropertyMapping(PropertyInfo.PropertyType, PropertyInfo.Name, GetTermUri(mappingContext.OntologyProvider));
        }
    }
}