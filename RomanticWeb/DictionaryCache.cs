using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RomanticWeb
{
    /// <summary>
    /// Basic in-memory cache
    /// </summary>
    internal class DictionaryCache : ICache
    {
        private readonly IDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();

        public bool Contains(string key)
        {
            return _dict.ContainsKey(key);
        }

        public T Retrieve<T>(string key)
        {
            return (T)_dict[key];
        }

        public void Store(string key, object data)
        {
            _dict[key] = data;
        }
    }
}