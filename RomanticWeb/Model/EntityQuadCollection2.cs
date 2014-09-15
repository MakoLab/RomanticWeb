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

        public int Count
        {
            get
            {
                return _quads.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId)
        {
            foreach (var entityQuad in _quads.Where(q => q.Object == Node.FromEntityId(entityId)))
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

        public bool Contains(EntityQuad item)
        {
            return _quads.Contains(item);
        }

        public void CopyTo(EntityQuad[] array, int arrayIndex)
        {
            _quads.CopyTo(array, arrayIndex);
        }

        public bool Remove(EntityQuad entityTriple)
        {
            _quads.Remove(entityTriple);
            return EntityQuads(entityTriple.EntityId).Remove(entityTriple);
        }

        IEnumerator<EntityQuad> IEnumerable<EntityQuad>.GetEnumerator()
        {
            return _quads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _quads.GetEnumerator();
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