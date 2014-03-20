using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;

namespace RomanticWeb.Mapping.Providers
{
    public interface IPropertyMappingProvider:ITermMappingProvider
    {
        PropertyInfo PropertyInfo { get; }

        Aggregation? Aggregation { get; }
    }
}