using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class OntologiesCollection:ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new OntologyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OntologyElement)element).Prefix;
        }
    }
}