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

        internal IEnumerable<T> Keys { get { return _indices.Select(item => item.Key); } }

        internal Index<T> this[int index] { get { return _indices[index]; } }

        internal Index<T> this[T key,int itemIndex]
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

        internal Index<T> Add(T key,int startAt,int length)
        {
            Index<T> result=null;
            int count=_indices.Count;
            if (count>0)
            {
                for (var index=0; index<count; index++)
                {
                    Index<T> itemIndex=_indices[index];
                    if (itemIndex.StartAt>startAt)
                    {
                        itemIndex.ItemIndex++;
                        for (int next=index+1; next<count; next++)
                        {
                            _indices[next].ItemIndex++;
                        }

                        Add(result=new Index<T>(index,key,startAt,length));
                        break;
                    }
                }

                if (result==null)
                {
                    Add(result=new Index<T>(count,key,startAt,length));
                }
            }
            else
            {
                Add(result=new Index<T>(0,key,startAt,length));
            }

            return result;
        }

        internal void Add(Index<T> newIndex)
        {
            _indices.Insert(newIndex.ItemIndex,newIndex);
            _lastSearchedIndex=0;
        }

        internal void Set(T key,int itemIndex,int length)
        {
            Set(0,key,itemIndex,length);
        }

        internal void Set(int startIndex,T key,int itemIndex,int length)
        {
            int totalChange=0;
            int count=_indices.Count;
            for (int index=startIndex; index<count; index++)
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
                    item.ItemIndex=index;
                }
            }
        }

        internal IEnumerable<Index<T>> Shift(T key)
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
                    item.ItemIndex=index;
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

        internal Index<T> Remove(T key,int itemIndex)
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
                    item.ItemIndex=index;
                }
            }

            return removed;
        }

        internal void Remove(T key)
        {
            Set(key,FirstPossible,0);
        }

        internal bool ContainsKey(T key)
        {
            return _indices.Any(item => Object.Equals(item.Key,key));
        }

        internal void Clear()
        {
            _indices.Clear();
            _lastSearchedIndex=0;
        }
    }
}