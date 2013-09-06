using ImpromptuInterface.Dynamic;

namespace RomanticWeb
{
    public class Entity : ImpromptuDictionary
    {
        private readonly EntityId _entityId;

        public Entity(EntityId entityId)
        {
            _entityId = entityId;
        }

        protected internal EntityId Id
        {
            get { return _entityId; }
        }
    }
}
