using System;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithExplicitConverters:EntityMap<IEntityWithExplicitConverters>
    {
        public EntityWithExplicitConverters()
        {
            Property(p => p.Property).Term.Is(new Uri("urn:not:important")).ConvertWith<BooleanConverter>();
            Collection(p => p.Collection).Term.Is(new Uri("urn:not:important")).ConvertElementsWith<BooleanConverter>();
        }
    }
}