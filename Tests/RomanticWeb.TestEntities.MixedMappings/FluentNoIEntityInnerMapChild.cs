using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class FluentNoIEntityInnerMapChild : FluentNoIEntityInnerMapParent
    {
        public string ChildProperty { get; set; }

        public class ChildMap : EntityMap<FluentNoIEntityInnerMapChild>
        {
            public ChildMap()
            {
                Property(e => e.ChildProperty).Term.Is(new Uri("urn:concrete:child"));
            }
        }
    }
}