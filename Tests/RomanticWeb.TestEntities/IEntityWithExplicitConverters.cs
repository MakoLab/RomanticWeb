using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithExplicitConverters:IEntity
    {
        [Property("urn:not:important",ConverterType=typeof(BooleanConverter))]
        int Property { get; }

        [Property("urn:not:important",ConverterType=typeof(BooleanConverter))]
        IList<string> Collection { get; }
    }
}