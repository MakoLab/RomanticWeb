using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithDictionaryMap:EntityMap<IEntityWithDictionary>
    {
        public EntityWithDictionaryMap()
        {
            Dictionary(e => e.SettingsDefault).Term.Is("magi","setting");
        }
    }
}