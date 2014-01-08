using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>Provides an observable dictionary behavior.</summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of item.</typeparam>
    public class ObservableDictionary<TKey,TValue>:IDictionary<TKey,TValue>,INotifyCollectionChanged,INotifyPropertyChanged
    {
        #region Fields
        private const string CountString="Count";
        private const string IndexerName="Item[]";
        private const string KeysName="Keys";
        private const string ValuesName="Values";

        private IDictionary<TKey,TValue> _dictionary;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public ObservableDictionary()
        {
            _dictionary=new Dictionary<TKey,TValue>();
        }

        /// <summary>Constructor with passed dictionary.</summary>
        /// <param name="dictionary">Source dictionary to use.</param>
        public ObservableDictionary(IDictionary dictionary)
        {
            _dictionary=new Dictionary<TKey,TValue>();
            foreach (DictionaryEntry item in dictionary)
            {
                _dictionary.Add(new KeyValuePair<TKey,TValue>((TKey)item.Key,(TValue)item.Value));
            }
        }

        /// <summary>Constructor with passed dictionary.</summary>
        /// <param name="dictionary">Source dictionary to use.</param>
        public ObservableDictionary(IDictionary<TKey,TValue> dictionary)
        {
            _dictionary=new Dictionary<TKey,TValue>(dictionary);
        }

        /// <summary>Constructor with item comparer passed.</summary>
        /// <param name="comparer">Equality comparer for dictionary items.</param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary=new Dictionary<TKey,TValue>(comparer);
        }

        /// <summary>Constructor with initial capacity passed.</summary>
        /// <param name="capacity">Initial capcity.</param>
        public ObservableDictionary(int capacity)
        {
            _dictionary=new Dictionary<TKey,TValue>(capacity);
        }

        /// <summary>Constructor with source dictionary and item comparer passed.</summary>
        /// <param name="dictionary">Source dictionary to use.</param>
        /// <param name="comparer">Equality comparer for dictionary items.</param>
        public ObservableDictionary(IDictionary<TKey,TValue> dictionary,IEqualityComparer<TKey> comparer)
        {
            _dictionary=new Dictionary<TKey,TValue>(dictionary,comparer);
        }

        /// <summary>Constructor with initial capcity and item comparer passed.</summary>
        /// <param name="capacity">Initial capcity.</param>
        /// <param name="comparer">Equality comparer for dictionary items.</param>
        public ObservableDictionary(int capacity,IEqualityComparer<TKey> comparer)
        {
            _dictionary=new Dictionary<TKey,TValue>(capacity,comparer);
        }
        #endregion

        #region Events
        /// <summary>Event for dictionary changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>Event for property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        /// <inheritdoc />
        public ICollection<TKey> Keys { get { return Dictionary.Keys; } }

        /// <inheritdoc />
        public ICollection<TValue> Values { get { return Dictionary.Values; } }

        /// <inheritdoc />
        public int Count { get { return Dictionary.Count; } }

        /// <inheritdoc />
        public bool IsReadOnly { get { return Dictionary.IsReadOnly; } }

        /// <summary>Gets the underlying dictionary.</summary>
        protected IDictionary<TKey,TValue> Dictionary { get { return _dictionary; } }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get { return Dictionary[key]; }
            set { Insert(key,value,false); }
        }
        #endregion

        #region IDictionary<TKey,TValue> Members
        /// <inheritdoc />
        public void Add(TKey key,TValue value)
        {
            Insert(key,value,true);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            TValue value;
            Dictionary.TryGetValue(key,out value);
            var removed=Dictionary.Remove(key);
            if (removed)
            {
                OnCollectionChanged();
            }

            return removed;
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key,out TValue value)
        {
            return Dictionary.TryGetValue(key,out value);
        }
        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members
        /// <inheritdoc />
        public void Add(KeyValuePair<TKey,TValue> item)
        {
            Insert(item.Key,item.Value,true);
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (Dictionary.Count>0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey,TValue> item)
        {
            return Dictionary.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey,TValue>[] array,int arrayIndex)
        {
            Dictionary.CopyTo(array,arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey,TValue> item)
        {
            return Remove(item.Key);
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }
        #endregion

        #region Public methods
        /// <summary>Adds items from another dictionary.</summary>
        /// <param name="items">Dictionary with new items.</param>
        public void AddRange(IDictionary<TKey,TValue> items)
        {
            if (items.Count>0)
            {
                if (Dictionary.Count>0)
                {
                    if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                    {
                        throw new ArgumentException("An item with the same key has already been added.");
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            Dictionary.Add(item);
                        }
                    }
                }
                else
                {
                    _dictionary=new Dictionary<TKey,TValue>(items);
                }

                OnCollectionChanged(NotifyCollectionChangedAction.Add,items.ToArray());
            }
        }
        #endregion

        #region Non-public methods
        /// <summary>Handler for property changes event.</summary>
        /// <param name="propertyName">Name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Insert(TKey key,TValue value,bool add)
        {
            TValue item;
            if (Dictionary.TryGetValue(key,out item))
            {
                if (add)
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }

                if (Equals(item,value))
                {
                    return;
                }

                Dictionary[key]=value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace,new KeyValuePair<TKey,TValue>(key,value),new KeyValuePair<TKey,TValue>(key,item));
            }
            else
            {
                Dictionary[key]=value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add,new KeyValuePair<TKey,TValue>(key,value));
            }
        }

        private void OnPropertyChanged()
        {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
        }

        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            if (CollectionChanged!=null)
            {
                CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action,KeyValuePair<TKey,TValue> changedItem)
        {
            OnPropertyChanged();
            if (CollectionChanged!=null)
            {
                CollectionChanged(this,new NotifyCollectionChangedEventArgs(action,changedItem));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action,KeyValuePair<TKey,TValue> newItem,KeyValuePair<TKey,TValue> oldItem)
        {
            OnPropertyChanged();
            if (CollectionChanged!=null)
            {
                CollectionChanged(this,new NotifyCollectionChangedEventArgs(action,newItem,oldItem));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action,IList newItems)
        {
            OnPropertyChanged();
            if (CollectionChanged!=null)
            {
                CollectionChanged(this,new NotifyCollectionChangedEventArgs(action,newItems));
            }
        }
        #endregion
    }
}