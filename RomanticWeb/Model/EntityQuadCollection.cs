using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Model
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class EntityQuadCollection : IEntityQuadCollection
    {
        private IList<EntityQuad> _quads = new List<EntityQuad>();
        private IDictionary<EntityId, int> _entities = new Dictionary<EntityId, int>();
        private IDictionary<string, ISet<EntityQuad>> _entityTypeQuads = new Dictionary<string, ISet<EntityQuad>>();
        private IndexCollection<string> _subjects = new IndexCollection<string>();
        private object _locker = new Object();

        public int Count
        {
            get
            {
                int result = 0;
                lock (_locker)
                {
                    result = _quads.Count;
                }

                return result;
            }
        }

        public IEnumerable<EntityId> Entities
        {
            get
            {
                IEnumerable<EntityId> result = new EntityId[0];
                lock (_locker)
                {
                    result = _entities.Keys;
                }

                return result;
            }
        }

        public int EntitiesCount
        {
            get
            {
                int result = 0;
                lock (_locker)
                {
                    result = _entities.Count;
                }

                return result;
            }
        }

        public IEnumerable<EntityQuad> Quads { get { return _quads; } }

        internal IndexCollection<string> Subjects { get { return _subjects; } }

        public IEnumerable<EntityQuad> this[EntityId entityId] { get { return GetEntities(MakeSubject(entityId)); } }

        /// <inheritdoc />
        IEnumerator<EntityId> IEnumerable<EntityId>.GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EntityId>)this).GetEnumerator();
        }

        public void Add(EntityQuad quad)
        {
            string key = MakeSubject(quad);
            Index<string> index = null;
            lock (_locker)
            {
                index = _subjects[key, IndexCollection<string>.FirstPossible];
            }

            AddInternal(quad, key, index);
        }

        public void Add(EntityId entityId, IEnumerable<EntityQuad> entityTriples)
        {
            string lastKey = null;
            Index<string> index = null;
            foreach (EntityQuad quad in entityTriples)
            {
                if (quad.EntityId != entityId)
                {
                    throw new ArgumentException(string.Format("All EntityTriples must reference EntityId {0} but {1} found", entityId, quad.EntityId), "entityTriples");
                }

                string key = MakeSubject(quad);
                if ((lastKey == null) || (key != lastKey))
                {
                    lastKey = key;
                    lock (_locker)
                    {
                        index = _subjects[key, IndexCollection<string>.FirstPossible];
                    }
                }

                index = AddInternal(quad, key, index);
            }
        }

        public void Remove(EntityQuad quad)
        {
            lock (_locker)
            {
                int indexOf = _quads.IndexOf(quad);
                if (indexOf != -1)
                {
                    string key = MakeSubject(quad);
                    _quads.RemoveAt(indexOf);
                    _subjects.Remove(key, indexOf);
                    UpdateStateAfterRemove(quad, key);
                }
            }
        }

        internal IEnumerable<EntityQuad> Remove(EntityId entityId)
        {
            IList<EntityQuad> result = new List<EntityQuad>();
            lock (_locker)
            {
                string key = MakeSubject(entityId);
                Index<string> index = _subjects[key, IndexCollection<string>.FirstPossible];
                if (index != null)
                {
                    int removedCount = 0;
                    while (removedCount < index.Length)
                    {
                        result.Add(_quads[index.StartAt]);
                        _quads.RemoveAt(index.StartAt);
                        _entities[entityId]--;
                        removedCount++;
                    }

                    _subjects.Remove(index.Key);
                    _entities.Remove(entityId);
                    _entityTypeQuads.Remove(key);
                }
            }

            return result;
        }

        public IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId)
        {
            IList<EntityQuad> result = new List<EntityQuad>();
            lock (_locker)
            {
                string key = null;
                bool lastIndexChanged = false;
                Index<string> lastIndex = null;
                for (int index = 0; index < _quads.Count; index++)
                {
                    if ((lastIndex == null) || (lastIndex.StartAt + lastIndex.Length <= index))
                    {
                        if ((lastIndex != null) && (lastIndexChanged))
                        {
                            _subjects.Set(lastIndex.Key, lastIndex.ItemIndex, lastIndex.Length);
                        }

                        key = MakeSubject(_quads[index].EntityId);
                        lastIndex = _subjects[key, index];
                        lastIndexChanged = false;
                    }

                    if (lastIndex != null)
                    {
                        EntityQuad quad = _quads[index];
                        if ((!quad.Object.IsLiteral) && (quad.Object.ToEntityId() == entityId))
                        {
                            result.Add(quad);
                            _quads.RemoveAt(index);
                            UpdateStateAfterRemove(quad, key);
                            lastIndex.Length--;
                            index--;
                            lastIndexChanged = true;
                        }
                    }
                }

                if ((lastIndex != null) && (lastIndexChanged))
                {
                    _subjects.Set(lastIndex.Key, lastIndex.ItemIndex, lastIndex.Length);
                }
            }

            return result;
        }

        public void Clear()
        {
            lock (_locker)
            {
                _quads.Clear();
                _subjects.Clear();
                _entities.Clear();
                _entityTypeQuads.Clear();
            }
        }

        public IEnumerable<EntityQuad> GetEntityTypeQuads(EntityId entityId)
        {
            ISet<EntityQuad> result;
            lock (_locker)
            {
                if (!_entityTypeQuads.TryGetValue(MakeSubject(entityId), out result))
                {
                    return new EntityQuad[0];
                }
            }

            return result;
        }

        [Obsolete("Make this private so that it's transparent to the caller")]
        public void SetEntityTypeQuads(EntityId entityId, IEnumerable<EntityQuad> typeQuads, Uri graphUri)
        {
            Index<string> index = null;
            string key = MakeSubject(entityId);
            bool added = false;
            lock (_locker)
            {
                index = _subjects[key, IndexCollection<string>.FirstPossible];
                ISet<EntityQuad> entityTypeQuads;
                if (_entityTypeQuads.TryGetValue(key, out entityTypeQuads))
                {
                    foreach (EntityQuad quad in entityTypeQuads)
                    {
                        int indexOf = _quads.IndexOf(quad);
                        if (indexOf != -1)
                        {
                            _quads.RemoveAt(indexOf);
                            if (_subjects.Remove(key, indexOf) != null)
                            {
                                index = null;
                            }

                            RemoveEntity(entityId);
                        }
                    }

                    entityTypeQuads.Clear();
                }
                else
                {
                    _entityTypeQuads[key] = new HashSet<EntityQuad>();
                }

                foreach (var quad in typeQuads.Distinct())
                {
                    added = true;
                    if (index == null)
                    {
                        index = _subjects.Add(key, _quads.Count, 1);
                        _quads.Add(quad);
                    }
                    else
                    {
                        _quads.Insert(index.StartAt + index.Length, quad);
                        _subjects.Set(index.ItemIndex, index.Key, index.StartAt, index.Length + 1);
                    }

                    _entityTypeQuads[index.Key].Add(quad);
                }
            }

            if (added)
            {
                AddEntity(entityId);
            }
        }

        public IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId)
        {
            IList<EntityQuad> result = new List<EntityQuad>();
            IList<BlankId> addedBlankNodes = new List<BlankId>();
            GetEntityQuads(result, addedBlankNodes, entityId);
            return result;
        }

        private void GetEntityQuads(IList<EntityQuad> result, IList<BlankId> addedBlankNodes, EntityId entityId)
        {
            IList<BlankId> blanksToAdd = new List<BlankId>();
            foreach (EntityQuad quad in this[entityId])
            {
                result.Add(quad);
                if (quad.Object.IsBlank)
                {
                    BlankId blankId = (BlankId)quad.Object.ToEntityId();
                    if (!addedBlankNodes.Contains(blankId))
                    {
                        addedBlankNodes.Add(blankId);
                        blanksToAdd.Add(blankId);
                    }
                }
            }

            foreach (BlankId blankId in blanksToAdd)
            {
                GetEntityQuads(result, addedBlankNodes, blankId);
            }
        }

        private Index<string> AddInternal(EntityQuad quad, string key, Index<string> index)
        {
            bool added = true;
            lock (_locker)
            {
                if (index == null)
                {
                    index = _subjects.Add(key, _quads.Count, 1);
                    _quads.Add(quad);
                }
                else
                {
                    if (!QuadExists(quad, index))
                    {
                        _quads.Insert(index.StartAt + index.Length, quad);
                        _subjects.Set(index.ItemIndex, index.Key, index.StartAt, index.Length + 1);
                    }
                    else
                    {
                        added = false;
                    }
                }
            }

            if (added)
            {
                UpdateStateAfterAdd(quad, index);
            }

            return index;
        }

        private string MakeSubject(EntityId entityId)
        {
            return (entityId is BlankId ? ((BlankId)entityId).Identifier : entityId.Uri.ToString());
        }

        private string MakeSubject(EntityQuad quad)
        {
            return MakeSubject(quad.Subject);
        }

        private string MakeSubject(Node node)
        {
            return (node.IsBlank ? node.BlankNode : node.Uri.ToString());
        }

        private IEnumerable<EntityQuad> GetEntities(string subject)
        {
            Index<string> index = null;
            lock (_locker)
            {
                index = _subjects[subject, IndexCollection<string>.FirstPossible];
            }

            if (index != null)
            {
                return _quads.Skip(index.StartAt).Take(index.Length);
            }
            else
            {
                return new EntityQuad[0];
            }
        }

        private bool QuadExists(EntityQuad quad, Index<string> index)
        {
            int endAt = index.StartAt + index.Length;
            for (int itemIndex = index.StartAt; itemIndex < endAt; itemIndex++)
            {
                if (_quads[itemIndex].Equals(quad))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateStateAfterAdd(EntityQuad quad, Index<string> index)
        {
            lock (_locker)
            {
                AddEntity(quad.EntityId);
                if (quad.Predicate.Uri.AbsoluteUri == Rdf.type.AbsoluteUri)
                {
                    ISet<EntityQuad> entityTypeQuads;
                    if (!_entityTypeQuads.TryGetValue(index.Key, out entityTypeQuads))
                    {
                        _entityTypeQuads[index.Key] = new HashSet<EntityQuad>();
                    }

                    _entityTypeQuads[index.Key].Add(quad);
                }
            }
        }

        private void UpdateStateAfterRemove(EntityQuad quad, string key)
        {
            lock (_locker)
            {
                RemoveEntity(quad.EntityId);
                if (quad.Predicate.Uri.AbsoluteUri == Rdf.type.AbsoluteUri)
                {
                    ISet<EntityQuad> entityTypeQuads;
                    if (_entityTypeQuads.TryGetValue(key, out entityTypeQuads))
                    {
                        _entityTypeQuads[key].Remove(quad);
                    }
                }
            }
        }

        private void AddEntity(EntityId entityId)
        {
            if (!_entities.ContainsKey(entityId))
            {
                _entities.Add(entityId, 1);
            }
            else
            {
                _entities[entityId]++;
            }
        }

        private void RemoveEntity(EntityId entityId)
        {
            if (_entities.ContainsKey(entityId))
            {
                _entities[entityId]--;
                if (_entities[entityId] == 0)
                {
                    _entities.Remove(entityId);
                }
            }
        }
    }
}