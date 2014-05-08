using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Linq.Model;

namespace RomanticWeb.Linq.Sparql
{
    internal class VisitedComponentCollection:IDictionary<IQueryComponent,KeyValuePair<int,int>>
    {
        private Dictionary<IQueryComponent,KeyValuePair<int,int>> _dictionary=new Dictionary<IQueryComponent,KeyValuePair<int,int>>();
        private StringBuilder _stringBuilder;

        internal VisitedComponentCollection(StringBuilder stringBuilder)
        {
            _stringBuilder=stringBuilder;
        }

        public ICollection<IQueryComponent> Keys { get { return _dictionary.Keys; } }

        public ICollection<KeyValuePair<int,int>> Values { get { return _dictionary.Values; } }

        public int Count { get { return _dictionary.Count; } }

        bool ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>.IsReadOnly { get { return ((ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>)_dictionary).IsReadOnly; } }

        public KeyValuePair<int,int> this[IQueryComponent key] { get { return _dictionary[key]; } set { _dictionary[key]=value; } }

        public void Add(IQueryComponent key,KeyValuePair<int,int> value)
        {
            _dictionary.Add(key,value);
        }

        public bool ContainsKey(IQueryComponent key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(IQueryComponent key)
        {
            if (_dictionary.ContainsKey(key))
            {
                KeyValuePair<int,int> visitedEntityConstrain=_dictionary[key];
                _stringBuilder=_stringBuilder.Remove(visitedEntityConstrain.Key,visitedEntityConstrain.Value);
                foreach (EntityConstrain itemKey in _dictionary.Keys.ToList())
                {
                    if (_dictionary[itemKey].Key>visitedEntityConstrain.Key)
                    {
                        _dictionary[itemKey]=new KeyValuePair<int,int>(_dictionary[itemKey].Key-visitedEntityConstrain.Key,_dictionary[itemKey].Value);
                    }
                }
            }

            return _dictionary.Remove(key);
        }

        public bool TryGetValue(IQueryComponent key,out KeyValuePair<int,int> value)
        {
            return _dictionary.TryGetValue(key,out value);
        }

        void ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>.Add(KeyValuePair<IQueryComponent,KeyValuePair<int,int>> item)
        {
            _dictionary.Add(item.Key,item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<IQueryComponent,KeyValuePair<int,int>> item)
        {
            return _dictionary.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>.CopyTo(KeyValuePair<IQueryComponent,KeyValuePair<int,int>>[] array,int arrayIndex)
        {
            ((ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>)_dictionary).CopyTo(array,arrayIndex);
        }

        bool ICollection<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>>.Remove(KeyValuePair<IQueryComponent,KeyValuePair<int,int>> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<IQueryComponent,KeyValuePair<int,int>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}