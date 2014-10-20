namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Provides dictionary mappings
    /// </summary>
    public interface IDictionaryMappingProvider : IPropertyMappingProvider
    {
        /// <summary>
        /// Gets the key mapping provider.
        /// </summary>
        IPredicateMappingProvider Key { get; }

        /// <summary>
        /// Gets the value mapping provider.
        /// </summary>
        IPredicateMappingProvider Value { get; }
    }
}