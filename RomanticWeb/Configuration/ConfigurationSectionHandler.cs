using System;
using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class ConfigurationSectionHandler:ConfigurationSection
    {
        private const string MetaGraphUriAttributeName="metaGraphUri";
        private const string BaseUrisName="baseUris";
        private const string MappingAssembliesElementName="mappingAssemblies";
        private const string OntologiesElementName="ontologies";

        public static ConfigurationSectionHandler Default
        {
            get
            {
                return (ConfigurationSectionHandler)ConfigurationManager.GetSection("romanticWeb")
                       ??new ConfigurationSectionHandler();
            }
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

        [ConfigurationProperty(MetaGraphUriAttributeName,IsRequired=true)]
        [UriValidator]
        public Uri MetaGraphUri
        {
            get { return (Uri)this[MetaGraphUriAttributeName]; }
            set { this[MetaGraphUriAttributeName] = value; }
        }
    }
}