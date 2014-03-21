using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class DerivedMap : EntityMap<IDerived>
    {
        public DerivedMap()
        {
            Property(e => e.MappedProperty1).Term.Is(new Uri("urn:override:fluent1"));
        }
    }
}