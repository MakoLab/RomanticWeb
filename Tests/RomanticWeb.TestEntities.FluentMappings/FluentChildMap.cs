using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Inheritance
{
    public class FluentChildMap : EntityMap<IFluentChild>
    {
        public FluentChildMap()
        {
            Property(child => child.SomeProperty).Term.Is(new Uri("urn:child:mapped"));
        }
    }
}