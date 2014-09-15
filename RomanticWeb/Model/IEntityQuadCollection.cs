using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    internal interface IEntityQuadCollection : ICollection<EntityQuad>
    {
        IEnumerable<EntityId> Entities { get; }

        IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId);

        void Add(EntityId entityId, IEnumerable<EntityQuad> entityQuads);

        IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId);
    }
}