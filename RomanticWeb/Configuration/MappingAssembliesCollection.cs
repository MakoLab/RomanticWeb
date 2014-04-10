using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class MappingAssembliesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingAssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingAssemblyElement)element).Assembly;
        }
    }
}