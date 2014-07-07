using System;
using System.Configuration;
using System.Xml;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Configuration of a ecntity context factory
    /// </summary>
    public class FactoryElement : ConfigurationElement
    {
        private const string NameAttributeName = "name";
        private const string MetaGraphUriAttributeName = "metaGraphUri";
        private const string BaseUrisName = "baseUris";
        private const string MappingAssembliesElementName = "mappingAssemblies";
        private const string OntologiesElementName = "ontologies";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ConfigurationProperty(NameAttributeName, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the mapping assemblies.
        /// </summary>
        [ConfigurationProperty(MappingAssembliesElementName)]
        [ConfigurationCollection(typeof(MappingAssembliesCollection))]
        public MappingAssembliesCollection MappingAssemblies
        {
            get { return (MappingAssembliesCollection)this[MappingAssembliesElementName]; }
            set { this[MappingAssembliesElementName] = value; }
        }

        /// <summary>
        /// Gets or sets the ontologies configuration element collection.
        /// </summary>
        [ConfigurationProperty(OntologiesElementName)]
        [ConfigurationCollection(typeof(OntologiesCollection))]
        public OntologiesCollection Ontologies
        {
            get { return (OntologiesCollection)this[OntologiesElementName]; }
            set { this[OntologiesElementName] = value; }
        }

        /// <summary>
        /// Gets or sets the base uri configuration element.
        /// </summary>
        [ConfigurationProperty(BaseUrisName)]
        public BaseUriElement BaseUris
        {
            get { return (BaseUriElement)this[BaseUrisName]; }
            set { this[BaseUrisName] = value; }
        }

        /// <summary>
        /// Gets or sets the meta graph URI.
        /// </summary>
        [ConfigurationProperty(MetaGraphUriAttributeName, IsRequired = true)]
        [UriValidator]
        public Uri MetaGraphUri
        {
            get { return (Uri)this[MetaGraphUriAttributeName]; }
            set { this[MetaGraphUriAttributeName] = value; }
        }
    }
}