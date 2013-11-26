using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Animals
{
    [Entity]
    public interface IUntypedAnimal
    {
        [Property("life", "name")]
        string Name { get; }
    }
}