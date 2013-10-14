using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	/// <summary>Base class for factories, which produce <see cref="Entity"/> instances.</summary>
	public class EntityContext:IEntityContext
	{
		private readonly IEntityStore _entityStore;
	    private readonly IEntitySource _entitySource;
	    private readonly IMappingsRepository _mappings;

        // todo: move catalog an container to a global location initiated at startup
        private readonly AssemblyCatalog _assemblyCatalog;
        private readonly CompositionContainer _container;
        private CompoundOntologyProvider _ontologyProvider;

	    public EntityContext(IMappingsRepository mappings, IEntitySource tripleStore)
            : this(mappings, new EntityStore(), tripleStore)
		{
		}

        /// <summary>Creates an instance of an entity context with given mappings and entity source.</summary>
        /// <param name="mappings">Information defining strongly typed interface mappings.</param>
        /// <param name="entityStore">Entity store to be used internally.</param>
        /// <param name="entitySource">Phisical entity data source.</param>
        internal EntityContext(IMappingsRepository mappings,IEntityStore entityStore,IEntitySource entitySource)
        {
            LogTo.Info("Creating entity context");
            _mappings=mappings;
            _entityStore=entityStore;
            _entitySource=entitySource;
            _ontologyProvider = new CompoundOntologyProvider();
            NodeConverter=new NodeConverter(this,entityStore);

            _assemblyCatalog=new AssemblyCatalog(GetType().Assembly);
            _container=new CompositionContainer(_assemblyCatalog,CompositionOptions.IsThreadSafe);
            _container.ComposeParts(NodeConverter);
        }

	    public IOntologyProvider OntologyProvider
	    {
	        get
	        {
	            return _ontologyProvider;
	        }

	        set
	        {
	            if (value!=null)
	            {
	                _ontologyProvider.OntologyProviders.Add(value);
	            }
	        }
	    }

	    public INodeConverter NodeConverter { get; set; }

        public void AddOntologyProvider(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider.OntologyProviders.Add(ontologyProvider);
        }

	    public IQueryable<Entity> AsQueryable()
		{
            return new EntityQueryable<Entity>(this,_mappings,OntologyProvider);
		}

		public IQueryable<T> AsQueryable<T>() where T:class,IEntity
		{
            return new EntityQueryable<T>(this,_mappings,OntologyProvider);
		}

		public Entity Load(EntityId entityId)
        {
            LogTo.Debug("Creating entity {0}",entityId);
			var entity=new Entity(entityId,this);

			foreach (var ontology in _ontologyProvider.Ontologies)
			{
			    var ontologyAccessor=new OntologyAccessor(_entityStore,entity,ontology,NodeConverter);
                _container.ComposeParts(ontologyAccessor);
			    entity[ontology.Prefix]=ontologyAccessor;
			}

			return entity;
		}

		public T Load<T>(EntityId entityId) where T:class,IEntity
		{
		    if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
			{
			    return (T)(IEntity)Load(entityId);
			}
		
            return EntityAs<T>(Load(entityId));
		}

	    public IEnumerable<Entity> Load(string sparqlConstruct)
		{
			return Load<Entity>(sparqlConstruct);
		}

		public IEnumerable<T> Load<T>(string sparqlConstruct) where T:class,IEntity
		{
			IList<T> entities=new List<T>();

            IEnumerable<Tuple<Node,Node,Node>> triples=_entitySource.GetNodesForQuery(sparqlConstruct);
			foreach (Node subject in triples.Select(triple => triple.Item1).Distinct())
			{
                entities.Add(Load<T>(subject.ToEntityId()));
			}

			return entities;
		}

	    internal void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}",entity.Id);
            _entitySource.LoadEntity(_entityStore,entity.Id);
        }

        internal T EntityAs<T>(Entity entity) where T:class,IEntity
        {
            LogTo.Trace("Wrapping entity {0} as {1}", entity.Id, typeof(T));
		    var proxy=new EntityProxy(_entityStore,entity,_mappings.MappingFor<T>(),NodeConverter);
            _container.ComposeParts(proxy);
            return proxy.ActLike<T>();
        }
	}
}