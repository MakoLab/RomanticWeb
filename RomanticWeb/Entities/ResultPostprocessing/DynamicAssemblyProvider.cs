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
            return Type.GetType("RomanticWeb.TestEntities.IEntityWithDictionary_SettingsDefault_Entry, RomanticWeb.TestEntities", true);
        }

        public Type GetOwnerType(IEntityMapping entity)
        {
            return Type.GetType("RomanticWeb.TestEntities.IEntityWithDictionary_SettingsDefault_Owner, RomanticWeb.TestEntities", true);
        }
    }
}