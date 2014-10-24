using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    internal interface IEntityMappingProviderWithHiddenProperties : IEntityMappingProvider
    {
        IEnumerable<IPropertyMappingProvider> HiddenProperties { get; } 
    }
}