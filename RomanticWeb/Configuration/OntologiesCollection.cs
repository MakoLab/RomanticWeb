using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// Collection of ontology configuration elements
    /// </summary>
    public class OntologiesCollection:ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="OntologyElement"/>
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new OntologyElement();
        }

        /// <summary>
        /// Returns <see cref="OntologyElement.Prefix"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OntologyElement)element).Prefix;
        }
    }
}