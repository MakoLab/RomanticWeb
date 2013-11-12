using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class HerbivoreMap : EntityMap<IHerbivore>
    {
        public HerbivoreMap()
        {
            Class.Is("life", "Herbivore");
            Collection(p => p.Diet).Predicate.Is("life", "plantEaten");
        }
    }
}