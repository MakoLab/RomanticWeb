using System;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// A mapping provider, which provides a predicate mapping
    /// </summary>
    public interface IPredicateMappingProvider : ITermMappingProvider
    {
        /// <summary> Gets or sets the type of the converter.</summary>
        Type ConverterType { get; set; }
    }
}