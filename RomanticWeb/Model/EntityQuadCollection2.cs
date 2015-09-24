using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    internal sealed class EntityQuadCollection2 : IEntityQuadCollection
    {
        private readonly bool _threadSafe;
        private readonly IDictionary<int, EntityQuad> _quads;
        private readonly IDictionary<EntityId, IDictionary<int, EntityQuad>> _entityQuads;
        private readonly IDictionary<Node, IDictionary<int, EntityQuad>> _subjectQuads;
        private readonly IDictionary<Tuple<Node, Node>, IDictionary<EntityQuad, EntityQuad>> _subjectPredicateQuads;
        private readonly IDictionary<Node, IDictionary<int, EntityId>> _objectIndex;

        internal EntityQuadCollection2(bool threadSafe)
        {
            if (_threadSafe = threadSafe)
            {
                _quads = new Dictionary<int, EntityQuad>();
                _entityQuads = new Dictionary<EntityId, IDictionary<int, EntityQuad>>();
                _subjectQuads = new Dictionary<Node, IDictionary<int, EntityQuad>>();
                _subjectPredicateQuads = new Dictionary<Tuple<Node, Node>, IDictionary<EntityQuad, EntityQuad>>();
                _objectIndex = new Dictionary<Node, IDictionary<int, EntityId>>();
            }
            else
            {
                _quads = new ConcurrentDictionary<int, EntityQuad>();
                _entityQuads = new ConcurrentDictionary<EntityId, IDictionary<int, EntityQuad>>();
                _subjectQuads = new ConcurrentDictionary<Node, IDictionary<int, EntityQuad>>();
                _subjectPredicateQuads = new ConcurrentDictionary<Tuple<Node, Node>, IDictionary<EntityQuad, EntityQuad>>();
                _objectIndex = new ConcurrentDictionary<Node, IDictionary<int, EntityId>>();
            }
        }

        public int Count { get { return _quads.Count; } }

        public bool IsReadOnly { get { return false; } }

        IEnumerable<EntityQuad> IEntityQuadCollection.this[EntityId entityId] { get { return EntityQuads(entityId).Values; } }

        IEnumerable<EntityQuad> IEntityQuadCollection.this[Node entityId] { get { return SubjectQuads(entityId).Values; } }

        IEnumerable<EntityQuad> IEntityQuadCollection.this[Node entityId, Node predicate] { get { return SubjectPredicateQuads(entityId, predicate).Values; } }

        public IEnumerable<EntityQuad> RemoveWhereObject(Node obj)
        {
            if (!_objectIndex.ContainsKey(obj))
            {
                return new EntityQuad[0];
            }

            var toRemove = from cotainingEntity in _objectIndex[obj]
                           from quadWithSubject in SubjectQuads(Node.FromEntityId(cotainingEntity.Value))
                           where quadWithSubject.Value.Object == obj
                           select quadWithSubject.Value;

            return toRemove.ToList().Where(Remove).ToList();
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
            var entity = GetEntityId(quad.EntityId);

            _quads[quad.GetHashCode()] = quad;
            EntityQuads(entity)[quad.GetHashCode()] = quad;
            EntityQuads(quad.EntityId)[quad.GetHashCode()] = quad;
            SubjectQuads(quad.Subject)[quad.GetHashCode()] = quad;
            SubjectPredicateQuads(quad.Subject, quad.Predicate)[quad] = quad;
            ObjectIndex(quad.Object)[quad.EntityId.GetHashCode()] = quad.EntityId;
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
            return _quads.ContainsKey(item.GetHashCode());
        }

        public void CopyTo(EntityQuad[] array, int arrayIndex)
        {
            _quads.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(EntityQuad entityTriple)
        {
            var entity = GetEntityId(entityTriple.EntityId);
            EntityQuads(entityTriple.EntityId).Remove(entityTriple.GetHashCode());
            EntityQuads(entity).Remove(entityTriple.GetHashCode());
            SubjectQuads(entityTriple.Subject).Remove(entityTriple.GetHashCode());
            SubjectPredicateQuads(entityTriple.Subject, entityTriple.Predicate).Remove(entityTriple);
            return _quads.Remove(entityTriple.GetHashCode());
        }

        IEnumerator<EntityQuad> IEnumerable<EntityQuad>.GetEnumerator()
        {
            return _quads.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _quads.Values.GetEnumerator();
        }

        private static EntityId GetEntityId(EntityId entityId)
        {
            var blankId = entityId as BlankId;
            while (blankId != null)
            {
                entityId = blankId.RootEntityId;
                blankId = entityId as BlankId;
            }

            return entityId;
        }

        private IDictionary<int, EntityQuad> EntityQuads(EntityId entityId)
        {
            if (!_entityQuads.ContainsKey(entityId))
            {
                _entityQuads[entityId] = (_threadSafe ? (IDictionary<int, EntityQuad>)new ConcurrentDictionary<int, EntityQuad>() : new Dictionary<int, EntityQuad>());
            }

            return _entityQuads[entityId];
        }

        private IDictionary<int, EntityQuad> SubjectQuads(Node subject)
        {
            if (!_subjectQuads.ContainsKey(subject))
            {
                _subjectQuads[subject] = (_threadSafe ? (IDictionary<int, EntityQuad>)new ConcurrentDictionary<int, EntityQuad>() : new Dictionary<int, EntityQuad>());
            }

            return _subjectQuads[subject];
        }

        private IDictionary<EntityQuad, EntityQuad> SubjectPredicateQuads(Node entityId, Node predicate)
        {
            var key = Tuple.Create(entityId, predicate);
            if (!_subjectPredicateQuads.ContainsKey(key))
            {
                _subjectPredicateQuads[key] = (_threadSafe ?
                    (IDictionary<EntityQuad, EntityQuad>)new ConcurrentDictionary<EntityQuad, EntityQuad>(LooseEntityQuadEqualityComparer.Instance) :
                    new Dictionary<EntityQuad, EntityQuad>(LooseEntityQuadEqualityComparer.Instance));
            }

            return _subjectPredicateQuads[key];
        }

        private IDictionary<int, EntityId> ObjectIndex(Node entityId)
        {
            if (!_objectIndex.ContainsKey(entityId))
            {
                _objectIndex[entityId] = (_threadSafe ? (IDictionary<int, EntityId>)new ConcurrentDictionary<int, EntityId>() : new Dictionary<int, EntityId>());
            }

            return _objectIndex[entityId];
        }
    }
}