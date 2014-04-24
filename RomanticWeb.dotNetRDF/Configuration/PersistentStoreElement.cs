using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using RomanticWeb.DotNetRDF.Configuration.StorageProviders;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF.Configuration
{
    public class PersistentStoreElement:StoreElement
    {
        private static readonly IDictionary<string,Func<StorageProviderElement>> ProviderElementFactories;
        private StorageProviderElement _storageProvider;

        static PersistentStoreElement()
        {
            ProviderElementFactories=new Dictionary<string,Func<StorageProviderElement>>();
            ProviderElementFactories["virtuosoManager"]=() => new VirtuosoManagerElement();
            ProviderElementFactories["allegroGraphConnector"]=() => new AllegroGraphConnectorElement();
            ProviderElementFactories["customProvider"]=() => new CustomProviderElement();
        }

        private StorageProviderElement StorageProvider
        {
            get
            {
                return _storageProvider;
            }

            set
            {
                if (_storageProvider!=null)
                {
                    throw new ConfigurationErrorsException("Cannot set two storage providers");
                }

                _storageProvider=value;
            }
        }

        public override ITripleStore CreateTripleStore()
        {
            return new PersistentTripleStore(StorageProvider.CreateStorageProvider());
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName,XmlReader reader)
        {
            if (ProviderElementFactories.ContainsKey(elementName))
            {
                StorageProviderElement providerElement=ProviderElementFactories[elementName].Invoke();
                providerElement.DeserializeElementForConfig(reader, false);
                StorageProvider=providerElement;
                return true;
            }

            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}