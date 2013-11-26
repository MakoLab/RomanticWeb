using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class UntypedAnimalMap : EntityMap<IUntypedAnimal>
    {
        public UntypedAnimalMap()
        {
            Property(p => p.Name).Term.Is("life", "name");
        }
    }
}