using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithDictionary:IEntity
    {
        [Dictionary("magi","setting")]
        IDictionary<string,object> SettingsDefault { get; }

        [Dictionary("urn:dictionary:property")]
        IDictionary<string,int> StringIntDictionary { get; }

        [Key("magi","key")]
        [Dictionary("urn:dictionary:customKey")]
        IDictionary<string,string> CustomQNameKeyDictionary { get; }

        [Key("http://magi/ontology#key")]
        [Dictionary("urn:dictionary:customKey")]
        IDictionary<string,string> CustomUriKeyDictionary { get; }
    }
}