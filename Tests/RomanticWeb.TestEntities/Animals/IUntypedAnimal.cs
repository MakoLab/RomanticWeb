using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    public interface IUntypedAnimal : IEntity
    {
        [Property("life", "name")]
        string Name { get; }
    }
}