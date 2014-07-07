using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Inheritance
{
    [Class("owl", "Thing")]
    public interface IInterface : IEntity
    {
        string Description { get; set; }
    }
}