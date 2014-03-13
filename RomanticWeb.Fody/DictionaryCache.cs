using System.Collections.Generic;

namespace RomanticWeb.Fody
{
    internal class DictionaryCache
    {
        public DictionaryCache()
        {
            Storage=new Dictionary<string,object>();
        }

        private Dictionary<string, object> Storage { get; set; }

        public bool Contains(string key)
        {
            return Storage.ContainsKey(key);
        }

        public T Retrieve<T>(string key)
        {
            return (T)Storage[key];
        }

        public void Store(string key, object data)
        {
            Storage[key] = data;
        }
    }
}