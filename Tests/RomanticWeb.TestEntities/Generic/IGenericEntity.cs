using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Generic
{
    public interface IGenericEntityWithAnyArgument<T>:IEntity
    {
        [Property("urn:generic:property")]
        T Property { get; }

        [Property("urn:generic:collection")]
        ICollection<T> Collection { get; } 
    }
}