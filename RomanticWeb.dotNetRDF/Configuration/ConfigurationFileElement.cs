using System.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration for a dotNetRDF configuration file
    /// </summary>
    public class ConfigurationFileElement:ConfigurationElement
    {
        private const string NameAttributeName = "name";
        private const string PathAttributeName = "path";
        private const string AutoConfigureAttributeName = "autoConfigure";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ConfigurationProperty(NameAttributeName)]
        public string Name
        {
            get { return (string)this[NameAttributeName]; }
            set { this[NameAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [ConfigurationProperty(PathAttributeName)]
        public string Path
        {
            get { return (string)this[PathAttributeName]; }
            set { this[PathAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether configuration should automatically configured.
        /// </summary>
        [ConfigurationProperty(AutoConfigureAttributeName, DefaultValue = true)]
        public bool AutoConfigure
        {
            get { return (bool)this[AutoConfigureAttributeName]; }
            set { this[AutoConfigureAttributeName] = value; }
        }
    }
}