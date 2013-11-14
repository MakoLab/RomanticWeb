using System.Collections.Generic;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// A typed entity, ie. one that exists in a rdf:type relation
    /// </summary>
    public interface ITypedEntity:IEntity
    {
        /// <summary>
        /// Gets or sets the entity's rdf classes
        /// </summary>
        IEnumerable<EntityId> Types { get; set; }
    }
}