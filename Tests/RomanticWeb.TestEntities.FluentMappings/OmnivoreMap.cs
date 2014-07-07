using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class OmnivoreMap : EntityMap<IOmnivore>
    {
        public OmnivoreMap()
        {
            Class.Is("life", "Omnivore");
            Collection(p => p.Diet).Term.Is("life", "plantEaten");
        }
    }
}