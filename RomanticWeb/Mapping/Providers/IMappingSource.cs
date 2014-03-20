using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    public interface IMappingSource
    {
        IEnumerable<IEntityMappingProvider> GetMappingProviders();
    }
}