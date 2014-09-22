using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration elements for dotNetRDF triple stores
    /// </summary>
    public class StoresCollection : ConfigurationElementCollection, IEnumerable<StoreElement>
    {
        private static readonly IDictionary<string, Func<StoresCollection, StoreElement>> StoreElementFactories;
        private readonly StoresConfigurationSection _parent;

        static StoresCollection()
        {
            StoreElementFactories = new Dictionary<string, Func<StoresCollection, StoreElement>>();
            StoreElementFactories["inMemory"] = self => new InMemoryStoreElement();
            StoreElementFactories["file"] = self => new FileStoreElement();
            StoreElementFactories["persistent"] = self => new PersistentStoreElement();
            StoreElementFactories["external"] = self => new ExternallyConfiguredStoreElement(self._parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoresCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent configuraion section.</param>
        public StoresCollection(StoresConfigurationSection parent)
        {
            _parent = parent;
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Not implemented
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets <see cref="StoreElement.Name"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StoreElement)element).Name;
        }

        /// <summary>
        /// Tries to deserialize a store element node
        /// </summary>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (StoreElementFactories.ContainsKey(elementName))
            {
                StoreElement storeElement = StoreElementFactories[elementName].Invoke(this);
                storeElement.DeserializeElementForConfig(reader, false);
                BaseAdd(storeElement);
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}