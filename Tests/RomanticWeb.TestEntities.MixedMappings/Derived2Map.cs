using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class Derived2Map : EntityMap<IDerivedLevel2>
    {
        public Derived2Map()
        {
            Property(e => e.MappedProperty1).Term.Is(new Uri("urn:override:fluent2"));
        }
    }
}