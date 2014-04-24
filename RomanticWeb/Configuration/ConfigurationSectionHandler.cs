using System.Configuration;

namespace RomanticWeb.Configuration
{
    public class ConfigurationSectionHandler:ConfigurationSection
    {
        private const string FactoryCollectionElementName="factories";

        public static ConfigurationSectionHandler Default
        {
            get
            {
                return (ConfigurationSectionHandler)ConfigurationManager.GetSection("romanticWeb")
                       ??new ConfigurationSectionHandler();
            }
        }

        [ConfigurationProperty(FactoryCollectionElementName)]
        [ConfigurationCollection(typeof(FactoriesCollection),AddItemName="factory")]
        public FactoriesCollection Factories
        {
            get { return (FactoriesCollection)this[FactoryCollectionElementName]; }
            set { this[FactoryCollectionElementName] = value; }
        }
    }
}