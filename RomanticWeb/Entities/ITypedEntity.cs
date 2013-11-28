using System.Collections.Generic;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.Entities
{
    /// <summary>A typed entity, ie. one that exists in a rdf:type relation.</summary>
    [Class("rdfs","Class")]
    public interface ITypedEntity:IEntity
    {
        /// <summary>Gets or sets the entity's rdf classes.</summary>
        [Collection("rdf", "type")]
        IEnumerable<EntityId> Types { get; set; }
    }
}