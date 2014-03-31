using System;
using System.Reflection;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Provides property mapping
    /// </summary>
    public interface IPropertyMappingProvider:ITermMappingProvider
    {
        /// <summary>
        /// Gets the mapped property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        PropertyInfo PropertyInfo { get; }

        Type ConverterType { get; set; }
    }
}