using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class HerbivoreMap : EntityMap<IHerbivore>
    {
        public HerbivoreMap()
        {
            Class.Is("life", "Herbivore");
            Collection(p => p.Diet).Term.Is("life", "plantEaten");
        }
    }
}