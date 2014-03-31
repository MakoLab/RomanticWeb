using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public interface IGenericParent<T>:IEntity
    {
        [Property("urn:open:mapping1")]
        T MappedProperty1 { get; }
        
        [Property("urn:open:mapping2")]
        T MappedProperty2 { get; }

        T UnMappedProperty { get; }

        [Property("urn:open:mapping1")]
        ICollection<T> GenericProperty { get; }
        
        string NonGenericProperty { get; }
    }
}
