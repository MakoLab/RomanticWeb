using System.Configuration;
using System.Linq;

namespace RomanticWeb.Configuration
{
    /// <summary>
    /// A collection of factory configuration elements
    /// </summary>
    public class FactoriesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the <see cref="FactoryElement"/> with the specified name.
        /// </summary>
        public new FactoryElement this[string factoryName]
        {
            get
            {
                return this.OfType<FactoryElement>().Single(e => e.Name == factoryName);
            }
        }

        /// <summary>
        /// Creates a new <see cref="FactoryElement"/>
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryElement();
        }

        /// <summary>
        /// Gets <see cref="FactoryElement.Name"/>
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactoryElement)element).Name;
        }
    }
}