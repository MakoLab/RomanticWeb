using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Source for mapping providers
    /// </summary>
    public interface IMappingProviderSource
    {
        /// <summary>
        /// Gets the mapping providers.
        /// </summary>
        IEnumerable<IEntityMappingProvider> GetMappingProviders();
    }
}