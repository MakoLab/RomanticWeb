using System.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class ConfigurationFilesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationFileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationFileElement)element).Name;
        }
    }
}