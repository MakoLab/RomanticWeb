namespace RomanticWeb
{
    public abstract class EntityFactoryBase<TTripleSource> : IEntityFactory
    {
        private readonly IOntologyProvider _ontologyProvider;

        protected EntityFactoryBase(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider = ontologyProvider;
        }

        public Entity Create(EntityId entityId)
        {
            Entity entity = CreateInternal(entityId);

            foreach (var ontology in _ontologyProvider.Ontologies)
            {
                entity[ontology.Prefix] = CreatePredicateAccessor(entity);
            }

            return entity;
        }

        protected abstract PredicateAccessor<TTripleSource> CreatePredicateAccessor(Entity entity);

        protected abstract Entity CreateInternal(EntityId entityId);
    }
}