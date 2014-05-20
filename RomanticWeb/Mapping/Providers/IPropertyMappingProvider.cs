using System;
using System.Reflection;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>Provides property mapping.</summary>
    public interface IPropertyMappingProvider:ITermMappingProvider
    {
        /// <summary>Gets the mapped property.</summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary> Gets or sets the type of the converter.</summary>
        Type ConverterType { get; set; }
    }
}