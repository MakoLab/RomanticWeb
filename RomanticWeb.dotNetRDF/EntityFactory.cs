namespace RomanticWeb.dotNetRDF
{
    public class EntityFactory : IEntityFactory
    {
        private readonly IOntologyProvider _ontologyProvider;

        public EntityFactory(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider = ontologyProvider;
        }

        public Entity Create(EntityId entityId)
        {
            return new Entity(entityId);
        }
    }
}
