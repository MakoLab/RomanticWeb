using System.Collections.Generic;
using System.Dynamic;
using ImpromptuInterface;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Base class for factories, which produce <see cref="Entity"/> instances
    /// </summary>
    public abstract class EntityFactoryBase : IEntityFactory
    {
        private readonly IMappingProvider _mappings;
        private readonly IOntologyProvider _ontologyProvider;

        protected EntityFactoryBase(IMappingProvider mappings, IOntologyProvider ontologyProvider)
        {
            _mappings = mappings;
            _ontologyProvider = new DefaultOntologiesProvider(ontologyProvider);
        }

        /// <summary>
        /// Creates a new instance of an entity
        /// </summary>
        public Entity Create(EntityId entityId)
        {
            var entity = new Entity(entityId, this);
            IDictionary<string, object> typeCheckerExpando = new ExpandoObject();

            foreach (var ontology in _ontologyProvider.Ontologies)
            {
                var source = CreateTriplesSourceForOntology();
                entity[ontology.Prefix] = new OntologyAccessor(source, entityId, ontology, new RdfNodeConverter(this));
                typeCheckerExpando[ontology.Prefix] = new TypeCheckerAccessor(entity, ontology);
            }
            entity["IsA"] = typeCheckerExpando;

            return entity;
        }

        public T Create<T>(EntityId entityId) where T : class
        {
            return EntityAs<T>(Create(entityId));
        }

        internal TEntity EntityAs<TEntity>(IEntity entity) where TEntity : class
        {
            var triplesSource = CreateTriplesSourceForEntity(_mappings.MappingFor<TEntity>());
            return new EntityProxy<TEntity>(triplesSource, entity.Id, _mappings.MappingFor<TEntity>(), new RdfNodeConverter(this)).ActLike<TEntity>();
        }

        protected abstract ITriplesSource CreateTriplesSourceForEntity<TEntity>(IMapping<TEntity> mappingFor);

        protected abstract ITriplesSource CreateTriplesSourceForOntology();
    }
}