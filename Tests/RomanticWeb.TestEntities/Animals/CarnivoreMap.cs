using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class CarnivoreMap : EntityMap<ICarnivore>
    {
        public CarnivoreMap()
        {
            Class.Is("life", "Carnivore");
            Collection(p => p.Prey).Term.Is("life","animalEaten");
        }
    }
}