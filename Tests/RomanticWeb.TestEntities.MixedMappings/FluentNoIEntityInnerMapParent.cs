using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class FluentNoIEntityInnerMapParent
    {
        public string ParentProperty { get; set; }

        public class Map : EntityMap<FluentNoIEntityInnerMapParent>
        {
            public Map()
            {
                Property(e => e.ParentProperty).Term.Is(new Uri("urn:concrete:parent"));
            }
        }
    }
}