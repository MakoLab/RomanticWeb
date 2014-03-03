using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    public class NamedGraphSelectionParameters
    {
        public NamedGraphSelectionParameters(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping propertyMapping)
        {
            EntityId = entityId;
            EntityMapping = entityMapping;
            PropertyMapping = propertyMapping;
        }

        public EntityId EntityId { get; private set; }

        public IEntityMapping EntityMapping { get; private set; }

        public IPropertyMapping PropertyMapping { get; private set; }
    }
}