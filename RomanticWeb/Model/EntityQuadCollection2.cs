using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Model
{
    internal sealed class EntityQuadCollection2 : IEntityQuadCollection
    {
        private static readonly Node UriNode = Node.ForUri(Rdf.type);

        private readonly ISet<EntityQuad> _quads = new HashSet<EntityQuad>();
        private readonly IDictionary<EntityId, ISet<EntityQuad>> _entityQuads = new Dictionary<EntityId, ISet<EntityQuad>>();
        private readonly IDictionary<EntityId, ISet<EntityQuad>> _entityTypeQuads = new Dictionary<EntityId, ISet<EntityQuad>>();

        public IEnumerable<EntityId> Entities
        {
            get
            {
                return _entityQuads.Keys;
            }
        }

        public IEnumerable<EntityQuad> Quads
        {
            get
            {
                return _quads;
            }
        }

        public IEnumerable<EntityQuad> this[EntityId entityId]
        {
            get
            {
                return EntityQuads(entityId);
            }
        }

        public IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId)
        {
            foreach (var entityQuad in Quads.Where(q => q.Object == Node.FromEntityId(entityId)))
            {
                Remove(entityQuad);
                yield return entityQuad;
            }
        }

        public IEnumerable<EntityQuad> GetEntityTypeQuads(EntityId entityId)
        {
            return EntityTypeQuads(entityId);
        }

        public void Add(EntityId entityId, IEnumerable<EntityQuad> entityQuads)
        {
            foreach (var entityQuad in entityQuads)
            {
                Add(entityQuad);
            }
        }

        public IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId)
        {
            return EntityQuads(entityId);
        }

        public void SetEntityTypeQuads(EntityId entityId, IEnumerable<EntityQuad> entityQuads, Uri graphUri)
        {
            foreach (var entityQuad in entityQuads)
            {
                Add(entityQuad);
                EntityTypeQuads(entityQuad.EntityId).Add(entityQuad);
            }
        }

        public void Add(EntityQuad quad)
        {
            _quads.Add(quad);
            EntityQuads(quad.EntityId).Add(quad);
        }

        public void Clear()
        {
            _quads.Clear();
            _entityQuads.Clear();
        }

        public void Remove(EntityQuad entityTriple)
        {
            _quads.Remove(entityTriple);
            EntityQuads(entityTriple.EntityId).Remove(entityTriple);
        }

        public IEnumerator<EntityId> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ISet<EntityQuad> EntityQuads(EntityId entityId)
        {
            if (!_entityQuads.ContainsKey(entityId))
            {
                _entityQuads[entityId] = new HashSet<EntityQuad>();
            }

            return _entityQuads[entityId];
        }

        private ISet<EntityQuad> EntityTypeQuads(EntityId entityId)
        {
            if (!_entityTypeQuads.ContainsKey(entityId))
            {
                _entityTypeQuads[entityId] = new HashSet<EntityQuad>();
            }

            return _entityTypeQuads[entityId];
        }
    }
}