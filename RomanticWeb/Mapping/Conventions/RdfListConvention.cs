using System.Collections.Generic;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class RdfListConvention:ICollectionConvention
    {
        public bool ShouldApply(ICollectionMappingProvider target)
        {
            return !target.Aggregation.HasValue
                && target.PropertyInfo.PropertyType.IsGenericType
                && target.PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
        }

        public void Apply(ICollectionMappingProvider target)
        {
            target.StoreAs=StoreAs.RdfList;
        }
    }
}