using System.Configuration;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class StoresConfigurationSection:ConfigurationSection
    {
        private const string StoresCollectionElementName = "stores";
        private const string ConfigurationFilesElementName = "dnrConfigurationFiles";

        public static StoresConfigurationSection Default
        {
            get
            {
                return (StoresConfigurationSection)ConfigurationManager.GetSection("romanticWeb.dotNetRDF")
                       ??new StoresConfigurationSection();
            }
        }

        public StoresCollection Stores { get; set; }

        [ConfigurationProperty(ConfigurationFilesElementName)]
        [ConfigurationCollection(typeof(ConfigurationFilesCollection))]
        public ConfigurationFilesCollection ConfigurationFiles
        {
            get { return (ConfigurationFilesCollection)this[ConfigurationFilesElementName]; }
            set { this[ConfigurationFilesElementName] = value; }
        }

        public ITripleStore CreateStore(string name)
        {
            return Stores.Single(store => store.Name==name).CreateTripleStore();
        }

        internal IConfigurationLoader OpenConfiguration(string name)
        {
            var configurationFile=ConfigurationFiles.Cast<ConfigurationFileElement>().FirstOrDefault(c => c.Name==name);

            if (configurationFile != null)
            {
                return new ConfigurationLoader(configurationFile.Path, configurationFile.AutoConfigure);
            }

            throw new ConfigurationErrorsException(string.Format("Configuration '{0}' wasn't found",name));
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            if (elementName==StoresCollectionElementName)
            {
                var storesCollection=new StoresCollection(this);
                storesCollection.Deserialize(reader);
                Stores=storesCollection;
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}