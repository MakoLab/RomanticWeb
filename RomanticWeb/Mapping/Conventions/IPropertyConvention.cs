using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// A contract for implementing property conventions
    /// </summary>
    public interface IPropertyConvention:IConvention<IPropertyMappingProvider>
    {
    }
}