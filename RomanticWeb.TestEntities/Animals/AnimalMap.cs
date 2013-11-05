using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class AnimalMap:EntityMap<IAnimal>
    {
        public AnimalMap()
        {
            Class.Is("life", "Animal");
            Property(p => p.Name).Predicate.Is("life","name");
        }
    }
}