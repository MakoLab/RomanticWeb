using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Model
{
    internal sealed class EntityQuadCollection:IEnumerable<EntityId>
    {
        private IList<EntityQuad> _quads=new List<EntityQuad>();
        private IDictionary<EntityId,int> _entities=new Dictionary<EntityId,int>();
        private IDictionary<string,IList<EntityQuad>> _entityTypeQuads=new Dictionary<string,IList<EntityQuad>>();
        private IndexCollection<string> _subjects=new IndexCollection<string>();
        private object _locker=new Object();

        internal int Count
        {
            get
            {
                int result=0;
                lock (_locker)
                {
                    result=_quads.Count;
                }

                return result;
            }
        }

        internal IEnumerable<EntityId> Entities
        {
            get
            {
                IEnumerable<EntityId> result=new EntityId[0];
                lock (_locker)
                {
                    result=_entities.Keys;
                }

                return result;
            }
        }

        internal int EntitiesCount
        {
            get
            {
                int result=0;
                lock (_locker)
                {
                    result=_entities.Count;
                }

                return result;
            }
        }

        internal IEnumerable<EntityQuad> Quads { get { return _quads; } }

        internal IndexCollection<string> Subjects { get { return _subjects; } }

        internal IEnumerable<EntityQuad> this[EntityId entityId] { get { return GetEntities(MakeSubject(entityId)); } }

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

        internal void Add(EntityQuad quad)
        {
            string key=MakeSubject(quad);
            Index<string> index=null;
            lock (_locker)
            {
                index=_subjects[key,IndexCollection<string>.FirstPossible];
            }

            AddInternal(quad,key,index);
        }

        internal void Add(EntityId entityId,IEnumerable<EntityQuad> entityTriples)
        {
            string lastKey=null;
            Index<string> index=null;
            foreach (EntityQuad quad in entityTriples)
            {
                if (quad.EntityId!=entityId)
                {
                    throw new ArgumentException(string.Format("All EntityTriples must reference EntityId {0} but {1} found",entityId,quad.EntityId),"entityTriples");
                }

                string key=MakeSubject(quad);
                if ((lastKey==null)||(key!=lastKey))
                {
                    lastKey=key;
                    lock (_locker)
                    {
                        index=_subjects[key,IndexCollection<string>.FirstPossible];
                    }
                }

                index=AddInternal(quad,key,index);
            }
        }

        internal void Remove(EntityQuad quad)
        {
            lock (_locker)
            {
                int indexOf=indexOf=_quads.IndexOf(quad);
                if (indexOf!=-1)
                {
                    _quads.RemoveAt(indexOf);
                    _subjects.Remove(MakeSubject(quad),indexOf);
                    if ((--_entities[quad.EntityId])==0)
                    {
                        _entities.Remove(quad.EntityId);
                    }
                }
            }
        }

        internal IEnumerable<EntityQuad> Remove(EntityId entityId)
        {
            IList<EntityQuad> result=new List<EntityQuad>();
            Index<string> index=_subjects[MakeSubject(entityId),IndexCollection<string>.FirstPossible];
            if (index!=null)
            {
                int removedCount=0;
                while (removedCount<index.Length)
                {
                    result.Add(_quads[index.StartAt]);
                    _quads.RemoveAt(index.StartAt);
                    _entities[entityId]--;
                    removedCount++;
                }

                _subjects.Remove(index.Key);
                if (_entities[entityId]==0)
                {
                    _entities.Remove(entityId);
                }
            }

            return result;
        }

        internal IEnumerable<EntityQuad> RemoveWhereObject(EntityId entityId)
        {
            IList<EntityQuad> result=new List<EntityQuad>();
            string key=null;
            bool lastIndexChanged=false;
            Index<string> lastIndex=null;
            for (int index=0; index<_quads.Count; index++)
            {
                if ((lastIndex==null)||(lastIndex.StartAt+lastIndex.Length<=index))
                {
                    if ((lastIndex!=null)&&(lastIndexChanged))
                    {
                        _subjects.Set(lastIndex.Key,lastIndex.ItemIndex,lastIndex.Length);
                    }

                    key=MakeSubject(_quads[index].EntityId);
                    lastIndex=_subjects[key,index];
                    lastIndexChanged=false;
                }

                if (lastIndex!=null)
                {
                    EntityQuad quad=_quads[index];
                    if ((!quad.Object.IsLiteral)&&(quad.Object.ToEntityId()==entityId))
                    {
                        result.Add(quad);
                        _quads.RemoveAt(index);
                        lastIndex.Length--;
                        index--;
                        lastIndexChanged=true;
                    }
                }
            }

            if ((lastIndex!=null)&&(lastIndexChanged))
            {
                _subjects.Set(lastIndex.Key,lastIndex.ItemIndex,lastIndex.Length);
            }

            return result;
        }

        internal void Clear()
        {
            lock (_locker)
            {
                _quads.Clear();
                _subjects.Clear();
                _entities.Clear();
                _entityTypeQuads.Clear();
            }
        }

        internal IEnumerable<EntityQuad> GetEntityTypeQuads(EntityId entityId)
        {
            IList<EntityQuad> result;
            lock (_locker)
            {
                if (!_entityTypeQuads.TryGetValue(MakeSubject(entityId),out result))
                {
                    result=new EntityQuad[0];
                }
            }

            return result;
        }

        private Index<string> AddInternal(EntityQuad quad,string key,Index<string> index)
        {
            bool added=true;
            lock (_locker)
            {
                if (index==null)
                {
                    index=_subjects.Add(key,_quads.Count,1);
                    _quads.Add(quad);
                }
                else
                {
                    if (!QuadExists(quad,index))
                    {
                        _quads.Insert(index.StartAt+index.Length,quad);
                        _subjects.Set(index.ItemIndex,index.Key,index.StartAt,index.Length+1);
                    }
                    else
                    {
                        added=false;
                    }
                }
            }

            if (added)
            {
                UpdateState(quad,index);
            }

            return index;
        }

        private string MakeSubject(EntityId entityId)
        {
            return (entityId is BlankId?((BlankId)entityId).Identifier:entityId.Uri.ToString());
        }

        private string MakeSubject(EntityQuad quad)
        {
            return MakeSubject(quad.Subject);
        }

        private string MakeSubject(Node node)
        {
            return (node.IsBlank?node.BlankNode:node.Uri.ToString());
        }

        private IEnumerable<EntityQuad> GetEntities(string subject)
        {
            Index<string> index=null;
            lock (_locker)
            {
                index=_subjects[subject,IndexCollection<string>.FirstPossible];
            }

            if (index!=null)
            {
                return _quads.Skip(index.StartAt).Take(index.Length);
            }
            else
            {
                return new EntityQuad[0];
            }
        }

        private bool QuadExists(EntityQuad quad,Index<string> index)
        {
            int endAt=index.StartAt+index.Length;
            for (int itemIndex=index.StartAt; itemIndex<endAt; itemIndex++)
            {
                if (_quads[itemIndex].Equals(quad))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateState(EntityQuad quad,Index<string> index)
        {
            lock (_locker)
            {
                if (!_entities.ContainsKey(quad.EntityId))
                {
                    _entities.Add(quad.EntityId,1);
                }
                else
                {
                    _entities[quad.EntityId]++;
                }

                if (quad.Predicate.Uri.AbsoluteUri==Rdf.type.AbsoluteUri)
                {
                    IList<EntityQuad> entityTypeQuads;
                    if (!_entityTypeQuads.TryGetValue(index.Key,out entityTypeQuads))
                    {
                        _entityTypeQuads[index.Key]=entityTypeQuads=new List<EntityQuad>();
                    }

                    _entityTypeQuads[index.Key].Add(quad);
                }
            }
        }
    }
}