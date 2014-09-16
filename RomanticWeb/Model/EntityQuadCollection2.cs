using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    internal sealed class EntityQuadCollection2 : IEntityQuadCollection
    {
        private readonly ISet<EntityQuad> _quads = new HashSet<EntityQuad>();
        private readonly IDictionary<EntityId, ISet<EntityQuad>> _entityQuads = new Dictionary<EntityId, ISet<EntityQuad>>();
        private readonly IDictionary<Node, ISet<EntityQuad>> _subjectQuads = new Dictionary<Node, ISet<EntityQuad>>();
        private readonly IDictionary<Tuple<Node, Node>, ISet<EntityQuad>> _subjectPredicateQuads = new Dictionary<Tuple<Node, Node>, ISet<EntityQuad>>();
        private readonly IDictionary<Node, ISet<EntityId>> _objectIndex = new Dictionary<Node, ISet<EntityId>>(); 

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

        IEnumerable<EntityQuad> IEntityQuadCollection.this[EntityId entityId]
        {
            get
            {
                return EntityQuads(entityId);
            }
        }

        IEnumerable<EntityQuad> IEntityQuadCollection.this[Node entityId]
        {
            get
            {
                return SubjectQuads(entityId);
            }
        }

        IEnumerable<EntityQuad> IEntityQuadCollection.this[Node entityId, Node predicate]
        {
            get
            {
                return SubjectPredicateQuads(entityId, predicate);
            }
        }

        public IEnumerable<EntityQuad> RemoveWhereObject(Node obj)
        {
            if (!_objectIndex.ContainsKey(obj))
            {
                yield break;
            }

            var toRemove = from cotainingEntity in _objectIndex[obj]
                           from quadWithSubject in SubjectQuads(Node.FromEntityId(cotainingEntity))
                           where quadWithSubject.Object == obj
                           select quadWithSubject;

            foreach (var entityQuad in toRemove.ToList())
            {
                Remove(entityQuad);
                yield return entityQuad;
            }
        }

        public void Add(EntityId entityId, IEnumerable<EntityQuad> entityQuads)
        {
            foreach (var entityQuad in entityQuads)
            {
                Add(entityQuad);
            }
        }

        public void Add(EntityQuad quad)
        {
            _quads.Add(quad);
            EntityQuads(quad.EntityId).Add(quad);
            SubjectQuads(quad.Subject).Add(quad);
            SubjectPredicateQuads(quad.Subject, quad.Predicate).Add(quad);
            ObjectIndex(quad.Object).Add(quad.EntityId);
        }

        public void Clear()
        {
            _quads.Clear();
            _entityQuads.Clear();
            _subjectQuads.Clear();
            _subjectPredicateQuads.Clear();
            _objectIndex.Clear();
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
            EntityQuads(entityTriple.EntityId).Remove(entityTriple);
            SubjectQuads(entityTriple.Subject).Remove(entityTriple);
            SubjectPredicateQuads(entityTriple.Subject, entityTriple.Predicate).Remove(entityTriple);
            return _quads.Remove(entityTriple);
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

        private ISet<EntityQuad> SubjectQuads(Node subject)
        {
            if (!_subjectQuads.ContainsKey(subject))
            {
                _subjectQuads[subject] = new HashSet<EntityQuad>();
            }

            return _subjectQuads[subject];
        }

        private ISet<EntityQuad> SubjectPredicateQuads(Node entityId, Node predicate)
        {
            var key = Tuple.Create(entityId, predicate);
            if (!_subjectPredicateQuads.ContainsKey(key))
            {
                _subjectPredicateQuads[key] = new HashSet<EntityQuad>();
            }

            return _subjectPredicateQuads[key];
        }

        private ISet<EntityId> ObjectIndex(Node entityId)
        {
            if (!_objectIndex.ContainsKey(entityId))
            {
                _objectIndex[entityId] = new HashSet<EntityId>();
            }

            return _objectIndex[entityId];
        }
    }
}