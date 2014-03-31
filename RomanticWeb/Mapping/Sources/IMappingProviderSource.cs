using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Sources
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