using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Collection of mapping assemblies configuration elements
    /// </summary>
    public class MappingAssembliesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="MappingAssemblyElement"/>
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingAssemblyElement();
        }

        /// <summary>
        /// Gets <see cref="MappingAssemblyElement.Assembly"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingAssemblyElement)element).Assembly;
        }
    }
}