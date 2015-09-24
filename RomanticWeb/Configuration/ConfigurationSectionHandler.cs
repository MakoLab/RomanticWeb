using System.Configuration;

namespace RomanticWeb.Configuration
{
    /// <summary>Configuration section for RomanticWeb.</summary>
    public class ConfigurationSectionHandler : ConfigurationSection
    {
        private const string FactoryCollectionElementName = "factories";

        /// <summary>Gets the configuration from default configutarion section.</summary>
        public static ConfigurationSectionHandler Default
        {
            get
            {
                return (ConfigurationSectionHandler)ConfigurationManager.GetSection("romanticWeb") ?? new ConfigurationSectionHandler();
            }
        }

        /// <summary>Gets or sets the collection of factory configurations.</summary>
        [ConfigurationProperty(FactoryCollectionElementName)]
        [ConfigurationCollection(typeof(FactoriesCollection), AddItemName = "factory")]
        public FactoriesCollection Factories
        {
            get { return (FactoriesCollection)this[FactoryCollectionElementName]; }
            set { this[FactoryCollectionElementName] = value; }
        }
    }
}