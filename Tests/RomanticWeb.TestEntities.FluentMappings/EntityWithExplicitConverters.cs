using System;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithExplicitConverters : EntityMap<IEntityWithExplicitConverters>
    {
        public EntityWithExplicitConverters()
        {
            Property(p => p.Property).Term.Is(new Uri("urn:not:important")).ConvertWith<BooleanConverter>();
            Collection(p => p.Collection).Term.Is(new Uri("urn:not:important")).ConvertElementsWith<BooleanConverter>();
            Dictionary(p => p.Dictionary).Term.Is(new Uri("urn:not:important"))
                                         .KeyPredicate.Is(new Uri("urn:key:converter"))
                                         .ValuePredicate.Is(new Uri("urn:value:converter"))
                                         .ConvertKeysWith<BooleanConverter>()
                                         .ConvertValuesWith<BooleanConverter>();
        }
    }
}