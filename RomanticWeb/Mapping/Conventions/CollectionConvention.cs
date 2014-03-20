using System.Collections.Generic;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class CollectionConvention:ICollectionConvention
    {
        public bool ShouldApply(ICollectionMappingProvider target)
        {
            var propertyType=target.PropertyInfo.PropertyType;

            return (target.StoreAs == StoreAs.Undefined) && propertyType.IsGenericType
                   &&(propertyType.GetGenericTypeDefinition()==typeof(IEnumerable<>)
                      ||propertyType.GetGenericTypeDefinition()==typeof(ICollection<>));
        }

        public void Apply(ICollectionMappingProvider target)
        {
            target.StoreAs=StoreAs.SimpleCollection;
        }
    }
}