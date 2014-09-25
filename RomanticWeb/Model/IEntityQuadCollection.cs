using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    internal interface IEntityQuadCollection : ICollection<EntityQuad>
    {
        /// <summary>
        /// Gets 
        /// </summary>
        IEnumerable<EntityQuad> this[EntityId entityId] { get; }
        
        IEnumerable<EntityQuad> this[Node entityId] { get; }

        IEnumerable<EntityQuad> this[Node entityId, Node predicate] { get; }

        IEnumerable<EntityQuad> RemoveWhereObject(Node entityId);

        void Add(EntityId entityId, IEnumerable<EntityQuad> entityQuads);
    }
}