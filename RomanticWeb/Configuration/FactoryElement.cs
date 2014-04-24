using System;
using System.Configuration;
using System.Xml;

namespace RomanticWeb.Configuration
{
    public class FactoryElement:ConfigurationElement
    {
        private const string NameAttributeName="name";
        private const string MetaGraphUriAttributeName="metaGraphUri";
        private const string BaseUrisName="baseUris";
        private const string MappingAssembliesElementName="mappingAssemblies";
        private const string OntologiesElementName="ontologies";

        [ConfigurationProperty(NameAttributeName,IsKey=true,IsRequired=true)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName]=value; }
        }

        [ConfigurationProperty(MappingAssembliesElementName)]
        [ConfigurationCollection(typeof(MappingAssembliesCollection))]
        public MappingAssembliesCollection MappingAssemblies
        {
            get { return (MappingAssembliesCollection)this[MappingAssembliesElementName]; }
            set { this[MappingAssembliesElementName] = value; }
        }

        [ConfigurationProperty(OntologiesElementName)]
        [ConfigurationCollection(typeof(OntologiesCollection))]
        public OntologiesCollection Ontologies
        {
            get { return (OntologiesCollection)this[OntologiesElementName]; }
            set { this[OntologiesElementName] = value; }
        }

        [ConfigurationProperty(BaseUrisName)]
        public BaseUriElement BaseUris
        {
            get { return (BaseUriElement)this[BaseUrisName]; }
            set { this[BaseUrisName] = value; }
        }

        [ConfigurationProperty(MetaGraphUriAttributeName, IsRequired = true)]
        [UriValidator]
        public Uri MetaGraphUri
        {
            get { return (Uri)this[MetaGraphUriAttributeName]; }
            set { this[MetaGraphUriAttributeName] = value; }
        }
    }
}