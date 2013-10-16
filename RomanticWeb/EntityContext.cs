using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using MethodCache.Attributes;
using NullGuard;
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
        #region Fields
        // todo: move catalog an container to a global location initiated at startup
        private readonly AssemblyCatalog _assemblyCatalog;
        private readonly CompositionContainer _container;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private IOntologyProvider _ontologyProvider;
        private INodeConverter _nodeConverter;
        #endregion

        #region Constructors
        /// <summary>Creates an instance of an entity context with given entity source.</summary>
        /// <param name="entitySource">Phisical entity data source.</param>
        public EntityContext(IEntitySource entitySource):this(new CompoundMappingsRepository(),new EntityStore(),entitySource)
		{
		}

        /// <summary>Creates an instance of an entity context with given mappings and entity source.</summary>
        /// <param name="mappings">Information defining strongly typed interface mappings.</param>
        /// <param name="entitySource">Phisical entity data source.</param>
        public EntityContext(IMappingsRepository mappings,IEntitySource entitySource):this(mappings,new EntityStore(),entitySource)
        {
        }

        /// <summary>Creates an instance of an entity context with given mappings and entity source.</summary>
        /// <param name="mappings">Information defining strongly typed interface mappings.</param>
        /// <param name="entityStore">Entity store to be used internally.</param>
        /// <param name="entitySource">Phisical entity data source.</param>
        internal EntityContext(IMappingsRepository mappings,IEntityStore entityStore,IEntitySource entitySource)
        {
            LogTo.Info("Creating entity context");
            _entityStore=entityStore;
            _entitySource=entitySource;
            _nodeConverter=new NodeConverter(this,entityStore);
            _mappings=mappings;
            _ontologyProvider=new DefaultOntologiesProvider();
            Cache = new DictionaryCache();

            _assemblyCatalog=new AssemblyCatalog(GetType().Assembly);
            _container=new CompositionContainer(_assemblyCatalog,CompositionOptions.IsThreadSafe);
            _container.ComposeParts(NodeConverter);
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets an ontology provider associated with this entity context.</summary>
	    public IOntologyProvider OntologyProvider
	    {
	        get
	        {
	            return _ontologyProvider;
	        }

            protected internal set
            {
                _ontologyProvider=value;
                _mappings.RebuildMappings(value);
            }
	    }

        public ICache Cache { get; set; }

        /// <summary>Gets or sets a node converter used by this entit context to transform RDF statements into strongly typed objects and values.</summary>
        public INodeConverter NodeConverter
        {
            get { return _nodeConverter; }
            set { _nodeConverter=value; }
        }
        #endregion

        #region Public methods

        /// <summary>Converts this context into a LINQ queryable data source.</summary>
        /// <returns>A LINQ querable data source.</returns>
	    public IQueryable<Entity> AsQueryable()
		{
            return new EntityQueryable<Entity>(this,_mappings,OntologyProvider);
		}

        /// <summary>Converts this context into a LINQ queryable data source of entities of given type.</summary>
        /// <typeparam name="T">Type of entities to work with.</typeparam>
        /// <returns>A LIQN queryable data source of entities of given type.</returns>
		public IQueryable<T> AsQueryable<T>() where T:class,IEntity
		{
            return new EntityQueryable<T>(this,_mappings,OntologyProvider);
		}

        /// <summary>Loads an entity from the underlying data source.</summary>
        /// <param name="entityId">IRI of the entity to be loaded.</param>
        /// <returns>Loaded entity.</returns>
        [Cache]
        [return:AllowNull]
        public Entity Load(EntityId entityId,bool checkIfExist=true)
        {
            LogTo.Debug("Loading entity {0}", entityId);

            if ((entityId is BlankId)||(!checkIfExist)||(_entitySource.EntityExist(entityId)))
			{
                return Create(entityId,false);
			}

            return null;
		}

        /// <summary>Loads a strongly typed entity from the underlying data source.</summary>
        /// <param name="entityId">IRI of the entity to be loaded.</param>
        /// <returns>Loaded entity.</returns>
        [return: AllowNull]
		public T Load<T>(EntityId entityId,bool checkIfExist=true) where T:class,IEntity
		{
            var entity=Load(entityId,checkIfExist);

            if (entity==null)
            {
                return null;
            }

            if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
			{
			    return (T)(IEntity)entity;
			}
		
            return EntityAs<T>(entity);
		}

	    public T Create<T>(EntityId entityId) where T : class,IEntity
        {
            if ((typeof(T) == typeof(IEntity)) || (typeof(T) == typeof(Entity)))
            {
                return (T)(IEntity)Create(entityId);
			}
		
            return EntityAs<T>(Create(entityId));
		}

	    public Entity Create(EntityId entityId)
        {
            LogTo.Debug("Creating entity {0}", entityId);
            return Create(entityId,true);
        }

        /// <summary>Loads multiple entities beeing a result of a SPARQL CONSTRUCT query.</summary>
        /// <param name="sparqlConstruct">SPARQL CONSTRUCT query to be used as a source of new entities.</param>
        /// <returns>Enumeration of entities loaded from the passed query.</returns>
	    public IEnumerable<Entity> Load(string sparqlConstruct)
		{
			return Load<Entity>(sparqlConstruct);
		}

        /// <summary>Loads multiple strongly typedentities beeing a result of a SPARQL CONSTRUCT query.</summary>
        /// <param name="sparqlConstruct">SPARQL CONSTRUCT query to be used as a source of new entities.</param>
        /// <returns>Enumeration of strongly typed entities loaded from the passed query.</returns>
		public IEnumerable<T> Load<T>(string sparqlConstruct) where T:class,IEntity
		{
			IList<T> entities=new List<T>();

            IEnumerable<Tuple<Node,Node,Node>> triples=_entitySource.GetNodesForQuery(sparqlConstruct);
			foreach (Node subject in triples.Select(triple => triple.Item1).Distinct())
			{
                entities.Add(Load<T>(subject.ToEntityId(),false));
			}

			return entities;
		}
        #endregion

        #region Non-public methods
        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
	    internal void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}",entity.Id);
            _entitySource.LoadEntity(_entityStore,entity.Id);
        }

        /// <summary>Transforms given entity into a strongly typed interface.</summary>
        /// <typeparam name="T">Type of the interface to transform given entity to.</typeparam>
        /// <param name="entity">Entity to be transformed.</param>
        /// <returns>Passed entity beeing a given interface.</returns>
        internal T EntityAs<T>(Entity entity) where T:class,IEntity
        {
            LogTo.Trace("Wrapping entity {0} as {1}", entity.Id, typeof(T));
		    var proxy=new EntityProxy(_entityStore,entity,_mappings.MappingFor<T>(),NodeConverter);
            _container.ComposeParts(proxy);
            return proxy.ActLike<T>();
        }

        private Entity Create(EntityId entityId, bool entityExists)
        {
            var entity=new Entity(entityId,this,entityExists);

            foreach (var ontology in _ontologyProvider.Ontologies)
            {
                var ontologyAccessor = new OntologyAccessor(_entityStore, entity, ontology, NodeConverter);
                _container.ComposeParts(ontologyAccessor);
                entity[ontology.Prefix] = ontologyAccessor;
            }

            return entity;
        }
        #endregion
	}
}