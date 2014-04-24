using System;
using System.Collections.Generic;
using VDS.RDF.Storage;

namespace RomanticWeb.DotNetRDF.Configuration.StorageProviders
{
    internal class AllegroGraphConnectorElement:StorageProviderElement
    {
        private const string BaseUriAttributeName = "baseUri";
        private const string StoreIdAttributeName = "storeID";
        private const string CatalogIdAttributeName = "catalogID";
        private const string UsernameAttributeName = "username";
        private const string PasswordAttributeName = "password";

        protected override Type ProviderType
        {
            get
            {
                return typeof(AllegroGraphConnector);
            }
        }

        protected override IEnumerable<string> ValidAttributes
        {
            get
            {
                yield return BaseUriAttributeName;
                yield return StoreIdAttributeName;
                yield return CatalogIdAttributeName;
                yield return UsernameAttributeName;
                yield return PasswordAttributeName;
            }
        }
    }
}