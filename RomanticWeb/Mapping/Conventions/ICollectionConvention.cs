using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// A contract for implementing collection conventions
    /// </summary>
    public interface ICollectionConvention:IConvention<ICollectionMappingProvider>
    {
    }
}