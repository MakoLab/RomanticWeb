using System.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Collection of dotNetRDF configuration file configurations
    /// </summary>
    public class ConfigurationFilesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Create a new instance of <see cref="ConfigurationFileElement"/>
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationFileElement();
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationFileElement.Name"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationFileElement)element).Name;
        }
    }
}