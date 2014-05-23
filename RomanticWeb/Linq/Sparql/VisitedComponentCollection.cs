using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Linq.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Linq.Sparql
{
    internal sealed class VisitedComponentCollection
    {
        private IList<Index<IQueryComponent>> _list=new List<Index<IQueryComponent>>();
        private StringBuilder _stringBuilder;

        internal VisitedComponentCollection(StringBuilder stringBuilder)
        {
            _stringBuilder=stringBuilder;
        }

        internal int Count { get { return _list.Count; } }

        internal bool Contains(IQueryComponent key)
        {
            return _list.Any(item => Object.Equals(key,item.Key));
        }

        internal void Add(IQueryComponent key,int startAt,int length)
        {
            _list.Add(new Index<IQueryComponent>(_list.Count-1,key,startAt,length));
        }

        internal void Remove(IQueryComponent key)
        {
            int totalChange=0;
            foreach (Index<IQueryComponent> index in _list)
            {
                if (!Object.Equals(index.Key,key))
                {
                    index.StartAt-=totalChange;
                }
                else
                {
                    _stringBuilder=_stringBuilder.Remove(index.StartAt,index.Length);
                    totalChange+=index.Length;
                }
            }
        }

        internal void Clear()
        {
            _list.Clear();
        }

        internal void Update(int startAt,int length)
        {
            foreach (Index<IQueryComponent> index in _list)
            {
                if (index.StartAt>=startAt)
                {
                    index.Length+=length;
                }
            }
        }
    }
}