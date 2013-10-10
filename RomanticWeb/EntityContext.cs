using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>Base class for factories, which produce <see cref="Entity"/> instances.</summary>
	public class EntityContext:IEntityContext
	{
		private readonly IEntityStore _entityStore;

	    private readonly IEntitySource _entitySource;

	    private readonly IMappingsRepository _mappings;

	    private IOntologyProvider _ontologyProvider;

	    public EntityContext(IMappingsRepository mappings, IEntitySource tripleStore)
            : this(mappings, new EntityStore(), tripleStore)
		{
		}

        internal EntityContext(IMappingsRepository mappings,IEntityStore entityStore,IEntitySource entitySource)
        {
            LogTo.Info("Creating entity context");
            _mappings = mappings;
            _entityStore=entityStore;
            _entitySource=entitySource;
            OntologyProvider = new DefaultOntologiesProvider();
            NodeProcessor=new NodeProcessor(this,entityStore);

            var assemblyCatalog=new AssemblyCatalog(GetType().Assembly);
            var container=new CompositionContainer(assemblyCatalog,CompositionOptions.IsThreadSafe);
            container.ComposeParts(NodeProcessor);
        }

        public IOntologyProvider OntologyProvider
        {
            get
            {
                return _ontologyProvider;
            }

            set
            {
                if (value is DefaultOntologiesProvider)
                {
                    _ontologyProvider=value;
                }
                else
                {
                    _ontologyProvider=new DefaultOntologiesProvider(value);
                }
            }
        }

        public INodeProcessor NodeProcessor { get; set; }

	    public IQueryable<Entity> AsQueryable()
		{
            return new EntityQueryable<Entity>(this, _mappings, OntologyProvider);
		}

		public IQueryable<T> AsQueryable<T>() where T:class,IEntity
		{
            return new EntityQueryable<T>(this, _mappings, OntologyProvider);
		}

		public Entity Create(EntityId entityId)
        {
            LogTo.Debug("Creating entity {0}", entityId);
			var entity=new Entity(entityId,this);

			foreach (var ontology in _ontologyProvider.Ontologies)
			{
			    entity[ontology.Prefix]=new OntologyAccessor(_entityStore,entity,ontology,NodeProcessor);
			}

			return entity;
		}

		public T Create<T>(EntityId entityId) where T:class,IEntity
		{
		    if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
			{
			    return (T)(IEntity)Create(entityId);
			}
		
            return EntityAs<T>(Create(entityId));
		}

	    public IEnumerable<Entity> Create(string sparqlConstruct)
		{
			return Create<Entity>(sparqlConstruct);
		}

		public IEnumerable<T> Create<T>(string sparqlConstruct) where T:class,IEntity
		{
			IList<T> entities=new List<T>();

            IEnumerable<Tuple<Node, Node, Node>> triples = _entitySource.GetNodesForQuery(sparqlConstruct);
			foreach (Node subject in triples.Select(triple => triple.Item1).Distinct())
			{
                entities.Add(Create<T>(subject.ToEntityId()));
			}

			return entities;
		}

	    internal void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}", entity.Id);
            _entitySource.LoadEntity(_entityStore,entity.Id);
        }

		internal T EntityAs<T>(Entity entity) where T : class,IEntity
        {
            LogTo.Trace("Wrapping entity {0} as {1}", entity.Id, typeof(T));
		    return new EntityProxy(_entityStore,entity,_mappings.MappingFor<T>(),NodeProcessor).ActLike<T>();
        }
	}
}