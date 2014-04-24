using System.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class ConfigurationFileElement:ConfigurationElement
    {
        private const string NameAttributeName = "name";
        private const string PathAttributeName = "path";
        private const string AutoConfigureAttributeName = "autoConfigure";

        [ConfigurationProperty(NameAttributeName)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName] = value; }
        }

        [ConfigurationProperty(PathAttributeName)]
        public string Path
        {
            get { return (string)this[PathAttributeName]; }
            set { this[PathAttributeName] = value; }
        }

        [ConfigurationProperty(AutoConfigureAttributeName, DefaultValue = true)]
        public bool AutoConfigure
        {
            get { return (bool)this[AutoConfigureAttributeName]; }
            set { this[AutoConfigureAttributeName] = value; }
        }
    }
}