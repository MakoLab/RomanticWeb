using System;
using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class OntologyElement:ConfigurationElement
    {
        private const string PrefixAttributeName="prefix";

        private const string UriAttributeName="uri";

        [ConfigurationProperty(PrefixAttributeName, IsRequired = true, IsKey = true)]
        public string Prefix
        {
            get { return (string)this[PrefixAttributeName]; }
            set { this[PrefixAttributeName] = value; }
        }

        [ConfigurationProperty(UriAttributeName, IsRequired = true, IsKey = true)]
        [UriValidator]
        public Uri Uri
        {
            get { return (Uri)this[UriAttributeName]; }
            set { this[UriAttributeName] = value; }
        }
    }
}