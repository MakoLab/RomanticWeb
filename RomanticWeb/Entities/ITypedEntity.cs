using System.Collections.Generic;

namespace RomanticWeb.Entities
{
    public interface ITypedEntity:IEntity
    {
        IEnumerable<EntityId> Types { get; set; }
    }
}