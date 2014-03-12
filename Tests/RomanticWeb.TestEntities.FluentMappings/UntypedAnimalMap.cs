using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class UntypedAnimalMap : EntityMap<IUntypedAnimal>
    {
        public UntypedAnimalMap()
        {
            Property(p => p.Name).Term.Is("life", "name");
        }
    }
}