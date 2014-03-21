using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class HidingMembersMap:EntityMap<IHidesMember>
    {
        public HidingMembersMap()
        {
            Property(e => e.MappedProperty2).Term.Is(new Uri("urn:hidden:fluent"));
        }
    }
}