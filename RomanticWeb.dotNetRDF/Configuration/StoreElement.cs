using System.Configuration;
using System.Xml;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration of a dotNetRDF triple store
    /// </summary>
    public abstract class StoreElement : ConfigurationElement, ITripleStoreFactory
    {
        private const string NameAttributeName = "name";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ConfigurationProperty(NameAttributeName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName] = value; }
        }

        /// <summary>
        /// Creates the triple store.
        /// </summary>
        public abstract ITripleStore CreateTripleStore();

        internal void DeserializeElementForConfig(XmlReader reader, bool serializeCollectionKey)
        {
            DeserializeElement(reader, serializeCollectionKey);
        }
    }
}