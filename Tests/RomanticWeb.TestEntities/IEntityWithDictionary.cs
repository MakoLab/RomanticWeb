using System.Collections.Generic;
using RomanticWeb.Collections;
using RomanticWeb.Collections.Mapping;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithDictionary:IEntity
    {
        [Dictionary("magi", "setting")]
        IDictionary<string, object> SettingsDefault { get; }
    }
#pragma warning disable

    public interface aIEntityWithDictionary_SettingsDefault : IDictionaryOwner<aIEntityWithDictionary_SettingsDefault_Entry, string, object>
    {
    }

    public interface aIEntityWithDictionary_SettingsDefault_Entry : IKeyValuePair<string, object>
    {
    }

    public class aIEntityWithDictionary_SettingsDefault_Entry_Mapping : DictionaryEntryMap<aIEntityWithDictionary_SettingsDefault_Entry,string,object>
    {
        protected override void SetupKeyProperty(TermPart<PropertyMap> term)
        {
            term.Is(Vocabularies.Rdf.predicate);
        }

        protected override void SetupValueProperty(TermPart<PropertyMap> term)
        {
            term.Is(Vocabularies.Rdf.@object);
        }
    }

    public class aIEntityWithDictionary_SettingsDefault_Mapping 
        : DictionaryOwnerMap<aIEntityWithDictionary_SettingsDefault,aIEntityWithDictionary_SettingsDefault_Entry,string,object>
    {
        protected override void SetupEntriesCollection(TermPart<CollectionMap> term)
        {
            term.Is("magi","setting");
        }
    }
#pragma warning restore
}