using System;
using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    internal interface IEntityQuadCollection : IEnumerable<EntityId>
    {
        IEnumerable<EntityId> Entities { get; }

        IEnumerable<EntityQuad> Quads { get; }

        IEnumerable<EntityQuad> this[EntityId entityId] { get; }

        IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId);

        IEnumerable<EntityQuad> GetEntityTypeQuads(EntityId entityId);

        void Add(EntityId entityId, IEnumerable<EntityQuad> entityQuads);

        IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId);

        void SetEntityTypeQuads(EntityId entityId, IEnumerable<EntityQuad> entityQuads, Uri graphUri);

        void Add(EntityQuad quad);

        void Clear();

        void Remove(EntityQuad entityTriple);
    }
}