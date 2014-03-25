using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;

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

        /// <summary>
        /// Gets the aggregation.
        /// </summary>
        /// <value>
        /// The aggregation.
        /// </value>
        Aggregation? Aggregation { get; }
    }
}