using System;
using System.Reflection;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>Provides property mapping.</summary>
    public interface IPropertyMappingProvider : IPredicateMappingProvider
    {
        /// <summary>Gets the mapped property.</summary>
        PropertyInfo PropertyInfo { get; }
    }
}