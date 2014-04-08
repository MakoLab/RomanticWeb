using System;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Provides class mappings
    /// </summary>
    public interface IClassMappingProvider:ITermMappingProvider
    {
        /// <summary>
        /// Gets the type, where this class was declared.
        /// </summary>
        Type DeclaringEntityType { get; }
    }
}