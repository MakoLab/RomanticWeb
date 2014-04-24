using System.Configuration;
using System.Xml;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public abstract class StoreElement:ConfigurationElement,ITripleStoreFactory
    {
        private const string NameAttributeName="name";

        [ConfigurationProperty(NameAttributeName,IsRequired=true,IsKey=true)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName]=value; }
        }

        public abstract ITripleStore CreateTripleStore();

        internal void DeserializeElementForConfig(XmlReader reader,bool serializeCollectionKey)
        {
            DeserializeElement(reader,serializeCollectionKey);
        }
    }
}