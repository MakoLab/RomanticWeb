using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    /// <summary>
    /// Entity factory implementation backed by a triple store
    /// </summary>
    public class EntityFactory : EntityFactoryBase<ITripleStore>
    {
        private readonly ITripleStore _tripeStore;

        /// <summary>
        /// Creates a new instance of <see cref="EntityFactory"/>
        /// </summary>
        /// <param name="tripeStore">Triples will be read from this triple store</param>
        /// <param name="ontologyProvider">Entites will expose dynamic members for the provided ontologies</param>
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
