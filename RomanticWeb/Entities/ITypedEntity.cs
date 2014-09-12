using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    /// <summary>A typed entity, ie. one that exists in a rdf:type relation.</summary>
    public interface ITypedEntity : IEntity
    {
        /// <summary>Gets the entity's rdf classes.</summary>
        [Collection("rdf", "type", StoreAs = StoreAs.SimpleCollection)]
        IList<EntityId> Types { get; set; }
    }
}