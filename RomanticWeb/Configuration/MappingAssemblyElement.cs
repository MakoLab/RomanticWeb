using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class MappingAssemblyElement:ConfigurationElement
    {
        private const string AssemlyAttributeName="assembly";

        [ConfigurationProperty(AssemlyAttributeName)]
        public string Assembly
        {
            get { return (string)this[AssemlyAttributeName]; }
            set { this[AssemlyAttributeName] = value; }
        }
    }
}