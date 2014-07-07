using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Inheritance
{
    public interface IAttributesParent : IEntity
    {
        [Property("urn:parent:mapped")]
        string SomeProperty { get; set; }

        [Property("urn:other:property")]
        string OtherProperty { get; set; }
    }
}