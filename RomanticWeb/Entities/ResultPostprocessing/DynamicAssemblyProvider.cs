using System;
using System.Reflection;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public class DynamicAssemblyProvider:IDictionaryPairTypeProvider
    {
        public Type GetEntryType(IPropertyMapping property)
        {
            return Type.GetType(string.Format("RomanticWeb.TestEntities.IEntityWithDictionary_{0}_Entry, RomanticWeb.TestEntities",property.Name), true);
        }

        public Type GetOwnerType(IPropertyMapping property)
        {
            return Type.GetType(string.Format("RomanticWeb.TestEntities.IEntityWithDictionary_{0}_Owner, RomanticWeb.TestEntities",property.Name), true);
        }
    }
}