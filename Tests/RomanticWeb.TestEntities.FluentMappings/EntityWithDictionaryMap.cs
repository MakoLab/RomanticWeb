using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithDictionaryMap:EntityMap<IEntityWithDictionary>
    {
        public EntityWithDictionaryMap()
        {
            MapDefaultQNameMappedDictionary();
            MapDefaultUriMappedDictionary();
            MapCustomQNameKeyDictionary();
            MapCustomUriKeyDictionary();
            MapCustomQNameValueDictionary();
            MapCustomUriValueDictionary();
            MapCustomKeyValueQNameDictionary();
            MapCustomKeyValueUriDictionary();
        }

        private void MapDefaultQNameMappedDictionary()
        {
            Dictionary(e => e.SettingsDefault).Term.Is("magi","setting");
        }

        private void MapDefaultUriMappedDictionary()
        {
            var dictionaryMap=Dictionary(e => e.StringIntDictionary);
            dictionaryMap.Term.Is(new Uri("urn:dictionary:property"));
        }

        private void MapCustomQNameKeyDictionary()
        {
            Dictionary(e => e.CustomQNameKeyDictionary)
                .Term.Is(new Uri("urn:dictionary:customKey"))
                .KeyPredicate.Is("magi","key");
        }

        private void MapCustomUriKeyDictionary()
        {
            Dictionary(e => e.CustomUriKeyDictionary)
                .Term.Is(new Uri("urn:dictionary:customKey"))
                .KeyPredicate.Is(new Uri("http://magi/ontology#key"));
        }

        private void MapCustomQNameValueDictionary()
        {
            Dictionary(e => e.CustomQNameValueDictionary)
                .Term.Is(new Uri("urn:dictionary:customValue"))
                .ValuePredicate.Is("magi","value");
        }

        private void MapCustomUriValueDictionary()
        {
            Dictionary(e => e.CustomUriValueDictionary)
                .Term.Is(new Uri("urn:dictionary:customValue"))
                .ValuePredicate.Is(new Uri("http://magi/ontology#value"));
        }

        private void MapCustomKeyValueQNameDictionary()
        {
            Dictionary(e => e.CustomKeyValueQNameDictionary)
                .Term.Is(new Uri("urn:dictionary:customKeyValue"))
                .KeyPredicate.Is("magi","key")
                .ValuePredicate.Is("magi","value");
        }

        private void MapCustomKeyValueUriDictionary()
        {
            Dictionary(e => e.CustomKeyValueUriDictionary)
                .Term.Is(new Uri("urn:dictionary:customKeyValue"))
                .KeyPredicate.Is(new Uri("http://magi/ontology#key"))
                .ValuePredicate.Is(new Uri("http://magi/ontology#value"));
        }
    }
}