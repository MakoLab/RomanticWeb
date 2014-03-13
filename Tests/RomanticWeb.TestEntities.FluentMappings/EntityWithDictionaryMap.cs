using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithDictionaryMap:EntityMap<IEntityWithDictionary>
    {
        public EntityWithDictionaryMap()
        {
            Dictionary(e => e.SettingsDefault).Term.Is("magi","setting");

            Dictionary(e => e.StringIntDictionary)
                .Term.Is(new Uri("urn:dictionary:property"));
            
            Dictionary(e => e.CustomQNameKeyDictionary)
                .Term.Is(new Uri("urn:dictionary:customKey"))
                .KeyPredicate.Is("magi","key");

            Dictionary(e => e.CustomUriKeyDictionary)
                .Term.Is(new Uri("urn:dictionary:customKey"))
                .KeyPredicate.Is(new Uri("http://magi/ontology#key"));
            
            Dictionary(e => e.CustomQNameValueDictionary)
                .Term.Is(new Uri("urn:dictionary:customValue"))
                .ValuePredicate.Is("magi","value");

            Dictionary(e => e.CustomUriValueDictionary)
                .Term.Is(new Uri("urn:dictionary:customValue"))
                .ValuePredicate.Is(new Uri("http://magi/ontology#value"));

            Dictionary(e => e.CustomKeyValueQNameDictionary)
                .Term.Is(new Uri("urn:dictionary:customKeyValue"))
                .KeyPredicate.Is("magi","key")
                .ValuePredicate.Is("magi","value");

            Dictionary(e => e.CustomKeyValueUriDictionary)
                .Term.Is(new Uri("urn:dictionary:customKeyValue"))
                .KeyPredicate.Is(new Uri("http://magi/ontology#key"))
                .ValuePredicate.Is(new Uri("http://magi/ontology#value"));
        }
    }
}