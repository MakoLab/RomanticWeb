using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class EntityFactory : EntityFactoryBase<ITripleStore>
    {
        private readonly ITripleStore _tripeStore;

        public EntityFactory(ITripleStore tripeStore, IOntologyProvider ontologyProvider) : base(ontologyProvider)
        {
            _tripeStore = tripeStore;
        }

        protected override PredicateAccessor<ITripleStore> CreatePredicateAccessor(Entity entity, Ontology ontology)
        {
            return new PredicateAccessor(_tripeStore, entity, ontology, this);
        }

        protected override Entity CreateInternal(EntityId entityId)
        {
            return new Entity(entityId);
        }
    }
}
