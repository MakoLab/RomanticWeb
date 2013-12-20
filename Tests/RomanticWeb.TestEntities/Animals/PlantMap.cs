using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Animals
{
    public class PlantMap : EntityMap<IPlant>
    {
        public PlantMap()
        {
            Class.Is("life","Plant");
        }
    }
}