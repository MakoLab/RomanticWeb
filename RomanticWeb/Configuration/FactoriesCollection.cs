using System.Configuration;
using System.Linq;
using System.Xml;

namespace RomanticWeb.Configuration
{
    public class FactoriesCollection:ConfigurationElementCollection
    {
        public FactoryElement this[string factoryName]
        {
            get
            {
                return this.OfType<FactoryElement>().Single(e => e.Name==factoryName);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactoryElement)element).Name;
        }
    }
}