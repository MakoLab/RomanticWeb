using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Providers
{
    public interface ICollectionMappingProvider:IPropertyMappingProvider
    {
        StoreAs StoreAs { get; set; }
    }
}