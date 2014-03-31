using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    [NullGuard(ValidationFlags.All^ValidationFlags.OutValues)]
    internal class RdfDictionary<TKey,TValue,TEntry,TOwner>:IDictionary<TKey,TValue>,IRdfDictionary
        where TEntry:class,IDictionaryEntry<TKey,TValue>
        where TOwner:class,IDictionaryOwner<TEntry,TKey,TValue>
    {
        private readonly TOwner _dictionaryOwner;
        private readonly IEntityContext _context;

        public RdfDictionary(EntityId ownerId,IEntityContext context)
        {
            _dictionaryOwner=context.Load<TOwner>(ownerId,false);
            _context=context;
        }

        public RdfDictionary(EntityId ownerId,IEntityContext context,IEnumerable<KeyValuePair<TKey,TValue>> existingDictionary)
            :this(ownerId,context)
        {
            foreach (var pair in existingDictionary)
            {
                Add(pair);
            }
        } 

        public int Count
        {
            get
            {
                return _dictionaryOwner.DictionaryEntries.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _dictionaryOwner.DictionaryEntries.Select(entry => entry.Key).ToList();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _dictionaryOwner.DictionaryEntries.Select(entry => entry.Value).ToList();
            }
        }

        IEnumerable<IEntity> IRdfDictionary.DictionaryEntries
        {
            get
            {
                return _dictionaryOwner.DictionaryEntries;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var entry=_dictionaryOwner.DictionaryEntries.SingleOrDefault(e => Equals(e.Key,key));

                if (entry==null)
                {
                    throw new KeyNotFoundException();
                }

                return entry.Value;
            }

            set
            {
                Add(key,value);
            }
        }

        public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
        {
            return new Enumerator(_dictionaryOwner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey,TValue> item)
        {
            Add(item.Key,item.Value);
        }

        public void Clear()
        {
            foreach (var entry in _dictionaryOwner.DictionaryEntries)
            {
                _context.Delete(entry.Id);
            }

            _dictionaryOwner.DictionaryEntries.Clear();
        }

        public bool Contains(KeyValuePair<TKey,TValue> item)
        {
            return _dictionaryOwner.DictionaryEntries.Any(pair => Equals(pair.Key,item.Key)&&Equals(pair.Value,item.Value));
        }

        public void CopyTo(KeyValuePair<TKey,TValue>[] array,int index)
        {
            int num=Count;
            var entryArray=_dictionaryOwner.DictionaryEntries.ToArray();
            for (int index1 = 0; index1 < num; ++index1)
            {
                array[index++]=new KeyValuePair<TKey,TValue>(entryArray[index1].Key,entryArray[index1].Value);
            }
        }

        public bool Remove(KeyValuePair<TKey,TValue> item)
        {
            var pair=GetPair(item.Key);
            if (pair==null)
            {
                return false;
            }

            if (!Equals(item.Value,pair.Value))
            {
                return false;
            }

            DeletePair(pair);
            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionaryOwner.DictionaryEntries.Any(e => Equals(e.Key, key));
        }

        public void Add(TKey key,TValue value)
        {
            var pair=_context.Create<TEntry>(new BlankId(_context.BlankIdGenerator.Generate(),_dictionaryOwner.Id));
            pair.Key=key;
            pair.Value=value;
            _dictionaryOwner.DictionaryEntries.Add(pair);
        }

        public bool Remove(TKey key)
        {
            var pair = GetPair(key);
            if (pair!=null)
            {
                DeletePair(pair);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key,out TValue value)
        {
            value=default(TValue);

            var pair=GetPair(key);
            if (pair!=null)
            {
                value=pair.Value;
                return true;
            }

            return false;
        }

        private void DeletePair(TEntry pair)
        {
            _context.Delete(pair.Id);
            _dictionaryOwner.DictionaryEntries.Remove(pair);
        }

        [return:AllowNull]
        private TEntry GetPair(TKey key)
        {
            return _dictionaryOwner.DictionaryEntries.SingleOrDefault(entry => Equals(entry.Key, key));
        }

        private class Enumerator:IEnumerator<KeyValuePair<TKey,TValue>>
        {
            private readonly IEnumerator<TEntry> _inner;

            public Enumerator(IDictionaryOwner<TEntry,TKey,TValue> dictionaryOwner)
            {
                _inner=dictionaryOwner.DictionaryEntries.GetEnumerator();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey,TValue>(_inner.Current.Key,_inner.Current.Value);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public bool MoveNext()
            {
                return _inner.MoveNext();
            }

            public void Reset()
            {
                _inner.Reset();
            }
        }
    }
}