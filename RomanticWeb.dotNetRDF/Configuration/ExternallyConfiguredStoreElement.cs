using System;
using System.Configuration;
using NullGuard;
using RomanticWeb.Configuration;
using VDS.RDF;
using VDS.RDF.Configuration;

namespace RomanticWeb.DotNetRDF.Configuration
{
    /// <summary>
    /// Configuration element for a triple store configured in a dotNetRDF configuration file
    /// </summary>
    public class ExternallyConfiguredStoreElement:StoreElement
    {
        private const string BnodeIdAttributeName = "blankNode";
        private const string UriAttributeName = "uri";
        private const string ConfigurationFileAttributeName = "dnrConfigurationfile";

        private readonly StoresConfigurationSection _stores;
        private IConfigurationLoader _configurationLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternallyConfiguredStoreElement"/> class.
        /// </summary>
        public ExternallyConfiguredStoreElement(StoresConfigurationSection stores)
        {
            _stores=stores;
        }

        /// <summary>
        /// Gets or sets the blank node identifier of configured store.
        /// </summary>
        [ConfigurationProperty(BnodeIdAttributeName)]
        public string BlankNodeIdentifier
        {
            get { return (string)this[BnodeIdAttributeName]; }
            set { this[BnodeIdAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the object URI of configured store.
        /// </summary>
        [ConfigurationProperty(UriAttributeName)]
        [UriValidator]
        public Uri ObjectUri
        {
            [return:AllowNull]
            get { return (Uri)this[UriAttributeName]; }
            set { this[UriAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the configuration as declared in the configuration section.
        /// </summary>
        [ConfigurationProperty(ConfigurationFileAttributeName, IsRequired = true)]
        public string ConfigurationName
        {
            get { return (string)this[ConfigurationFileAttributeName]; }
            set { this[ConfigurationFileAttributeName] = value; }
        }

        internal IConfigurationLoader ConfigurationLoader
        {
            get
            {
                if (_configurationLoader==null)
                {
                    _configurationLoader=_stores.OpenConfiguration(ConfigurationName);
                }

                return _configurationLoader;
            }

            set
            {
                _configurationLoader=value;
            }
        }

        /// <summary>
        /// Creates the triple store by loading it from the relevant configuration file.
        /// </summary>
        public override ITripleStore CreateTripleStore()
        {
            bool isUriSet=ObjectUri!=null;
            bool isBnodeSet=!string.IsNullOrWhiteSpace(BlankNodeIdentifier);

            if (isUriSet&&isBnodeSet)
            {
                throw new ConfigurationErrorsException("Cannot set both blank node and uri");
            }

            if (!(isBnodeSet||isUriSet))
            {
                throw new ConfigurationErrorsException("Either blank node or uri must be set");
            }

            if (isBnodeSet)
            {
                return ConfigurationLoader.LoadObject<ITripleStore>(BlankNodeIdentifier);
            }

            return ConfigurationLoader.LoadObject<ITripleStore>(ObjectUri);
        }
    }
}