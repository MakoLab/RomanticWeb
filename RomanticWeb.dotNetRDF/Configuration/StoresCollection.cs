using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class StoresCollection:ConfigurationElementCollection,IEnumerable<StoreElement>
    {
        private static readonly IDictionary<string, Func<StoresCollection, StoreElement>> StoreElementFactories;
        private readonly StoresConfigurationSection _parent;

        static StoresCollection()
        {
            StoreElementFactories=new Dictionary<string,Func<StoresCollection,StoreElement>>();
            StoreElementFactories["inMemory"] = self => new InMemoryStoreElement();
            StoreElementFactories["persistent"] = self => new PersistentStoreElement();
            StoreElementFactories["external"] = self => new ExternallyConfiguredStoreElement(self._parent);
        }

        public StoresCollection(StoresConfigurationSection parent)
        {
            _parent = parent;
        }

        IEnumerator<StoreElement> IEnumerable<StoreElement>.GetEnumerator()
        {
            foreach (var storeElement in this)
            {
                yield return (StoreElement)storeElement;
            }
        }

        internal void Deserialize(XmlReader reader)
        {
            DeserializeElement(reader, false);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StoreElement)element).Name;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName,System.Xml.XmlReader reader)
        {
            if (StoreElementFactories.ContainsKey(elementName))
            {
                StoreElement storeElement=StoreElementFactories[elementName].Invoke(this);
                storeElement.DeserializeElementForConfig(reader, false);
                BaseAdd(storeElement);
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}