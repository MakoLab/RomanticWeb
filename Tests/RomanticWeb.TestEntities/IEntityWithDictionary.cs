using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithDictionary : IEntity
    {
        [Dictionary("magi", "setting")]
        IDictionary<string, object> SettingsDefault { get; }

        [Dictionary("urn:dictionary:property")]
        IDictionary<string, int> StringIntDictionary { get; }

        [Key("magi", "key")]
        [Dictionary("urn:dictionary:customKey")]
        IDictionary<string, string> CustomQNameKeyDictionary { get; }

        [Key("http://magi/ontology#key")]
        [Dictionary("urn:dictionary:customKey")]
        IDictionary<string, string> CustomUriKeyDictionary { get; }

        [Value("magi", "value")]
        [Dictionary("urn:dictionary:customValue")]
        IDictionary<string, int> CustomQNameValueDictionary { get; }

        [Value("http://magi/ontology#value")]
        [Dictionary("urn:dictionary:customValue")]
        IDictionary<string, int> CustomUriValueDictionary { get; }

        [Key("magi", "key")]
        [Value("magi", "value")]
        [Dictionary("urn:dictionary:customKeyValue")]
        IDictionary<int, int> CustomKeyValueQNameDictionary { get; }

        [Key("http://magi/ontology#key")]
        [Value("http://magi/ontology#value")]
        [Dictionary("urn:dictionary:customKeyValue")]
        IDictionary<int, int> CustomKeyValueUriDictionary { get; set; }
    }
}