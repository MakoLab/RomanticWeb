using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml;
using ImpromptuInterface;
using VDS.RDF.Storage;

namespace RomanticWeb.DotNetRDF.Configuration.StorageProviders
{
    internal abstract class StorageProviderElement:ConfigurationElement
    {
        private readonly IDictionary<string,string> _constructorParameters=new Dictionary<string,string>();

        protected virtual IDictionary<string, string> ConstructorParameters
        {
            get
            {
                return _constructorParameters;
            }
        }

        protected abstract Type ProviderType { get; }

        protected virtual IEnumerable<string> ValidAttributes
        {
            get
            {
                yield break;
            }
        }

        public IStorageProvider CreateStorageProvider()
        {
            var ctorArguments=GetConstructorArguments();
            return Impromptu.InvokeConstructor(ProviderType,ctorArguments);
        }

        internal void DeserializeElementForConfig(XmlReader reader,bool serializeCollectionKey)
        {
            DeserializeElement(reader,serializeCollectionKey);
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (ValidAttributes.Contains(name))
            {
                HandleAttribute(name,value);
                return true;
            }

            return base.OnDeserializeUnrecognizedAttribute(name, value);
        }

        protected virtual void HandleAttribute(string name,string value)
        {
            ConstructorParameters.Add(name, value);
        }

        private ConstructorInfo GetBestMatchingConstructor()
        {
            var constructors=(from ctor in ProviderType.GetConstructors()
                              let parameters=ctor.GetParameters()
                              where parameters.Length==ConstructorParameters.Count
                              where parameters.All(p=>ConstructorParameters.ContainsKey(p.Name))
                              select ctor).ToArray();

            if (!constructors.Any())
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        "Type {0} doesn't contain a public constructor with {1} parameters",
                        ProviderType,
                        ConstructorParameters.Count));
            }

            if (constructors.Length==1)
            {
                return constructors.Single();
            }

            throw new ConfigurationErrorsException(string.Format("Multiple constructors matched on type {0}",ProviderType));
        }

        private object[] GetConstructorArguments()
        {
            ConstructorInfo constructor=GetBestMatchingConstructor();

            return (from param in constructor.GetParameters()
                    let element = ConstructorParameters[param.Name]
                    select Convert.ChangeType(element, param.ParameterType)).ToArray();
        }
    }
}