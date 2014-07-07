using System;
using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// An ontology configuration element
    /// </summary>
    public class OntologyElement : ConfigurationElement
    {
        private const string PrefixAttributeName = "prefix";
        private const string UriAttributeName = "uri";

        /// <summary>
        /// Gets or sets the ontology prefix.
        /// </summary>
        [ConfigurationProperty(PrefixAttributeName, IsRequired = true, IsKey = true)]
        public string Prefix
        {
            get { return (string)this[PrefixAttributeName]; }
            set { this[PrefixAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the ontology URI.
        /// </summary>
        [ConfigurationProperty(UriAttributeName, IsRequired = true, IsKey = true)]
        [UriValidator]
        public Uri Uri
        {
            get { return (Uri)this[UriAttributeName]; }
            set { this[UriAttributeName] = value; }
        }
    }
}