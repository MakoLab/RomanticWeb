using System;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Provides class mappings
    /// </summary>
    public interface IClassMappingProvider:ITermMappingProvider
    {
        Type DeclaringEntityType { get; }
    }
}