using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class OmnivoreMap : EntityMap<IOmnivore>
    {
        public OmnivoreMap()
        {
            Class.Is("life","Omnivore");
            Collection(p => p.Diet).Predicate.Is("life", "plantEaten");
        }
    }
}