using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Model
{
    internal sealed class IndexCollection<T>
    {
        internal const int FirstPossible=-1;

        private IList<Index<T>> _indices=new List<Index<T>>();
        private int _lastSearchedIndex=0;

        public IEnumerable<T> Keys { get { return _indices.Select(item => item.Key); } }

        public Index<T> this[T key,int itemIndex]
        {
            get
            {
                int count=_indices.Count;
                int lastSearchedIndex=_lastSearchedIndex;
                for (int index=_lastSearchedIndex; index<count; index++)
                {
                    Index<T> item=_indices[index];
                    if ((Object.Equals(item.Key,key))&&(item.Contains(itemIndex)))
                    {
                        _lastSearchedIndex=index;
                        return item;
                    }
                }

                if (lastSearchedIndex!=0)
                {
                    for (int index=0; index<lastSearchedIndex; index++)
                    {
                        Index<T> item=_indices[index];
                        if ((Object.Equals(item.Key,key))&&(item.Contains(itemIndex)))
                        {
                            _lastSearchedIndex=index;
                            return item;
                        }
                    }
                }

                return null;
            }

            set
            {
                if (value!=null)
                {
                    Set(key,itemIndex,value.Length);
                }
            }
        }

        public void Add(T key,int startAt,int length)
        {
            Add(new Index<T>(key,startAt,length));
        }

        public void Add(Index<T> newIndex)
        {
            _indices.Add(newIndex);
            _lastSearchedIndex=0;
        }

        public void Set(T key,int itemIndex,int length)
        {
            int totalChange=0;
            int count=_indices.Count;
            for (int index=0; index<count; index++)
            {
                Index<T> item=_indices[index];
                if ((Object.Equals(item.Key,key))&&(item.Contains(itemIndex)))
                {
                    totalChange+=length-item.Length;
                    item.Length=length;
                    if (item.Length==0)
                    {
                        _indices.RemoveAt(index);
                        count--;
                        index--;
                    }
                }
                else if (item.StartAt>=itemIndex)
                {
                    item.StartAt+=totalChange;
                }
            }
        }

        public IEnumerable<Index<T>> Shift(T key)
        {
            int totalChange=0;
            int count=_indices.Count;
            IList<Index<T>> result=new List<Index<T>>();
            for (int index=0; index<count; index++)
            {
                Index<T> item=_indices[index];
                if (!Object.Equals(item.Key,key))
                {
                    item.StartAt-=totalChange;
                }
                else
                {
                    result.Add(item);
                    _indices.RemoveAt(index);
                    count--;
                    index--;
                }
            }

            if (result.Count>0)
            {
                _lastSearchedIndex=0;
            }

            return result;
        }

        public Index<T> Remove(T key,int itemIndex)
        {
            Index<T> removed=null;
            int totalChange=0;
            int count=_indices.Count;
            for (int index=0; index<_indices.Count; index++)
            {
                Index<T> item=_indices[index];
                if ((Object.Equals(item.Key,key))&&(item.Contains(itemIndex)))
                {
                    totalChange+=1;
                    item.Length--;
                    if (item.Length==0)
                    {
                        _indices.RemoveAt(index);
                        removed=item;
                        index--;
                        count--;
                        _lastSearchedIndex=0;
                    }
                }
                else if (item.StartAt>=itemIndex)
                {
                    item.StartAt-=totalChange;
                }
            }

            return removed;
        }

        public bool ContainsKey(T key)
        {
            return _indices.Any(item => Object.Equals(item.Key,key));
        }

        public void Clear()
        {
            _indices.Clear();
            _lastSearchedIndex=0;
        }
    }
}