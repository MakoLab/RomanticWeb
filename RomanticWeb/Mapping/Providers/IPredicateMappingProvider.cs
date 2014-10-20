using System;

namespace RomanticWeb.Mapping.Providers
{
    public interface IPredicateMappingProvider : ITermMappingProvider
    {
        /// <summary> Gets or sets the type of the converter.</summary>
        Type ConverterType { get; set; }
    }
}