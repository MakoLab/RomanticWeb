using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>Base class for factories, which produce <see cref="Entity"/> instances.</summary>
	public class EntityContext:IEntityFactory
	{
		private readonly IEntityStore _entityStore;

	    private readonly ITripleStoreAdapter _tripleStore;

	    private readonly IMappingsRepository _mappings;

	    private readonly IOntologyProvider _ontologyProvider;

	    private readonly IDictionary<EntityId,bool> _entitiesState=new ConcurrentDictionary<EntityId,bool>();

        public EntityContext(IMappingsRepository mappings, IOntologyProvider ontologyProvider, ITripleStoreAdapter tripleStore)
            : this(mappings, ontologyProvider, new EntityStore(), tripleStore)
		{
		}

        internal EntityContext(IMappingsRepository mappings,IOntologyProvider ontologyProvider,IEntityStore entityStore,ITripleStoreAdapter tripleStore)
        {
            _mappings = mappings;
            _entityStore=entityStore;
            _tripleStore=tripleStore;
            _ontologyProvider = new DefaultOntologiesProvider(ontologyProvider);
        }

	    public IQueryable<Entity> AsQueryable()
		{
			return new EntityQueryable<Entity>(this,_mappings,_ontologyProvider);
		}

		public IQueryable<T> AsQueryable<T>() where T:class,IEntity
		{
			return new EntityQueryable<T>(this,_mappings,_ontologyProvider);
		}

		public Entity Create(EntityId entityId)
		{
			Entity entity=new Entity(entityId,this);

			foreach (var ontology in _ontologyProvider.Ontologies)
			{
				entity[ontology.Prefix]=new OntologyAccessor(_entityStore,entity,ontology,new RdfNodeConverter(this));
			}

            if (entityId is BlankId)
            {
                _entitiesState[entity.Id] = true;
            }

			return entity;
		}

		public T Create<T>(EntityId entityId) where T:class,IEntity
		{
		    if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
			{
			    return (T)(IEntity)Create(entityId);
			}
		
            return this.EntityAs<T>(this.Create(entityId));
		}

	    public IEnumerable<Entity> Create(string sparqlConstruct)
		{
			return Create<Entity>(sparqlConstruct);
		}

		public IEnumerable<T> Create<T>(string sparqlConstruct) where T:class,IEntity
		{
			IList<T> entities=new List<T>();

            IEnumerable<Tuple<RdfNode, RdfNode, RdfNode>> triples = _tripleStore.GetNodesForQuery(sparqlConstruct);
			foreach (RdfNode subject in triples.Select(triple => triple.Item1).Distinct())
			{
                entities.Add(Create<T>(subject.ToEntityId()));
			}

			return entities;
		}

        public void InitializeEnitity(IEntity entity)
        {
            if (!_entitiesState.ContainsKey(entity.Id))
            {
                _entitiesState[entity.Id]=false;
            }

            if (_entitiesState[entity.Id]==false)
            {
                _tripleStore.LoadEntity(_entityStore,entity.Id);
                _entitiesState[entity.Id]=true;
            }
        }

		internal T EntityAs<T>(Entity entity) where T : class,IEntity
		{
			return new EntityProxy(_entityStore, entity, _mappings.MappingFor<T>(), new RdfNodeConverter(this)).ActLike<T>();
		}
	}
}