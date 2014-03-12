using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class AnimalMap : EntityMap<IAnimal>
    {
        public AnimalMap()
        {
            Class.Is("life", "Animal");
            Property(p => p.Name).Term.Is("life", "name");
        }
    }
}