using System;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Inheritance.Specialized
{
    [Class("owl","Thing")]
    public interface IInterface:RomanticWeb.TestEntities.Inheritance.IInterface
    {
    }

    public class InterfaceMap:EntityMap<IInterface>
    {
        public InterfaceMap():base()
        {
            Property(instance => instance.Description).Term.Is(new Uri(RomanticWeb.Vocabularies.Owl.Thing.AbsoluteUri.Replace("#","/")+"#test"));
        }
    }
}