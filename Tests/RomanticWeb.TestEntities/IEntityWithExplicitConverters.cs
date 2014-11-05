using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithExplicitConverters : IEntity
    {
        [Property("urn:not:important", ConverterType = typeof(BooleanConverter))]
        int Property { get; }

        [Collection("urn:not:important", ConverterType = typeof(BooleanConverter))]
        IList<string> Collection { get; }

        [Dictionary("urn:not:important", KeyConverterType = typeof(BooleanConverter), ValueConverterType = typeof(BooleanConverter))]
        [Key("urn:key:predicate")]
        [Value("urn:value:predicate")]
        IDictionary<string, string> Dictionary { get; }
    }
}