using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Base class for factories, which produce <see cref="Entity"/> instances
    /// </summary>
    /// <typeparam name="TTripleSource">Type of RDF datasource, like graph of triple store</typeparam>
    public abstract class EntityFactoryBase : IEntityFactory
    {
        private readonly IOntologyProvider _ontologyProvider;

        protected EntityFactoryBase(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider = new DefaultOntologiesProvider(ontologyProvider);
        }

        /// <summary>
        /// Creates a new instance of an entity
        /// </summary>
        public Entity Create(EntityId entityId)
        {
            Entity entity = CreateInternal(entityId);
            IDictionary<string, object> typeCheckerExpando = new ExpandoObject();

            foreach (var ontology in _ontologyProvider.Ontologies)
            {
                entity[ontology.Prefix] = CreatePredicateAccessor(entity, ontology);
                typeCheckerExpando[ontology.Prefix] = new TypeCheckerAccessor(entity, ontology);
            }
            entity["IsA"] = typeCheckerExpando;

            return entity;
        }

        protected abstract PredicateAccessor CreatePredicateAccessor(Entity entity, Ontology ontology);

        protected abstract Entity CreateInternal(EntityId entityId);
    }
}