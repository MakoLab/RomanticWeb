using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class CarnivoreMap : EntityMap<ICarnivore>
    {
        public CarnivoreMap()
        {
            Class.Is("life", "Carnivore");
            Collection(p => p.Prey).Term.Is("life", "animalEaten");
        }
    }
}