using System;
using System.Collections.Generic;
using System.Configuration;
using VDS.RDF.Storage;

namespace RomanticWeb.DotNetRDF.Configuration.StorageProviders
{
    internal class VirtuosoManagerElement : StorageProviderElement
    {
        private const string ConnectionStringNameAttributeName = "connectionStringName";
        private const string ConnectionStringAttributeName = "connectionString";
        private const string ServerAttributeName = "server";
        private const string PortAttributeName = "port";
        private const string DatabaseAttributeName = "db";
        private const string UserAttributeName = "user";
        private const string PasswordAttributeName = "password";
        private const string TimeoutAttributeName = "timeout";

        protected override Type ProviderType
        {
            get
            {
                return typeof(VirtuosoManager);
            }
        }

        protected override IEnumerable<string> ValidAttributes
        {
            get
            {
                yield return ConnectionStringNameAttributeName;
                yield return ConnectionStringAttributeName;
                yield return PortAttributeName;
                yield return ServerAttributeName;
                yield return DatabaseAttributeName;
                yield return UserAttributeName;
                yield return PasswordAttributeName;
                yield return TimeoutAttributeName;
            }
        }

        protected override void HandleAttribute(string name, string value)
        {
            if (name == ConnectionStringNameAttributeName)
            {
                base.HandleAttribute(ConnectionStringAttributeName, ConfigurationManager.ConnectionStrings[value].ConnectionString);
                return;
            }

            base.HandleAttribute(name, value);
        }
    }
}