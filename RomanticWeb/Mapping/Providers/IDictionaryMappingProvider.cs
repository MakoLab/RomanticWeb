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
        ITermMappingProvider Key { get; }

        /// <summary>
        /// Gets the value mapping provider.
        /// </summary>
        ITermMappingProvider Value { get; }
    }
}