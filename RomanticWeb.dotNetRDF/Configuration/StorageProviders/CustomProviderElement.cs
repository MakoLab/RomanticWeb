using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RomanticWeb.DotNetRDF.Configuration.StorageProviders
{
    internal class CustomProviderElement : StorageProviderElement
    {
        private const string TypeAttributeName = "type";
        private const string ConstructorParametersElementName = "parameters";

        [ConfigurationProperty(TypeAttributeName, IsRequired = true)]
        [CallbackValidator(CallbackMethodName = "ValidateType", Type = typeof(Validators))]
        public string TypeName
        {
            get { return (string)this[TypeAttributeName]; }
            set { this[TypeAttributeName] = value; }
        }

        [ConfigurationProperty(ConstructorParametersElementName)]
        public KeyValueConfigurationCollection ConstructorParametersElement
        {
            get { return (KeyValueConfigurationCollection)this[ConstructorParametersElementName]; }
            set { this[ConstructorParametersElementName] = value; }
        }

        protected override IDictionary<string, string> ConstructorParameters
        {
            get
            {
                return (from KeyValueConfigurationElement element in ConstructorParametersElement
                        select element).ToDictionary(e => e.Key, e => e.Value);
            }
        }

        protected override Type ProviderType
        {
            get
            {
                return Type.GetType(TypeName);
            }
        }
    }
}