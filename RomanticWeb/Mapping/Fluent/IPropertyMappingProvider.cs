using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    internal interface IPropertyMappingProvider
    {
        IPropertyMapping GetMapping(MappingContext mappingContext);
    }
}