namespace RomanticWeb
{
    public class Entity
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
