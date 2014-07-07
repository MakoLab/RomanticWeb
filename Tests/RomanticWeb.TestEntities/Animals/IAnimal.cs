using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Class("life", "Animal")]
    public interface IAnimal : IEntity
    {
        [Property("life", "name")]
        string Name { get; }
    }
}