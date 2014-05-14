using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Mapping assembly configuration element
    /// </summary>
    public class MappingAssemblyElement:ConfigurationElement
    {
        private const string AssemlyAttributeName="assembly";

        /// <summary>
        /// Gets or sets the assembly name.
        /// </summary>
        [ConfigurationProperty(AssemlyAttributeName)]
        public string Assembly
        {
            get { return (string)this[AssemlyAttributeName]; }
            set { this[AssemlyAttributeName] = value; }
        }
    }
}