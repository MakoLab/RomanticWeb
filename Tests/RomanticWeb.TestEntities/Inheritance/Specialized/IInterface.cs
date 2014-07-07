using System;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Inheritance.Specialized
{
    [Class("owl", "Thing")]
    public interface IInterface : Inheritance.IInterface
    {
    }

    public class InterfaceMap : EntityMap<IInterface>
    {
        public InterfaceMap()
        {
            Property(instance => instance.Description).Term.Is(new Uri(Vocabularies.Owl.Thing.AbsoluteUri.Replace("#", "/") + "#test"));
        }
    }
}