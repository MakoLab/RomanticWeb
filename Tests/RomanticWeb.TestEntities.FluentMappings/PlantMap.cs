using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class PlantMap : EntityMap<IPlant>
    {
        public PlantMap()
        {
            Class.Is("life","Plant");
        }
    }
}