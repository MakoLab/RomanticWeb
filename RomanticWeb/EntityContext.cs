using System;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using MethodCache.Attributes;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb
{
    /// <summary>
    /// Creates a new instance of <see cref="EntityContext"/>
    /// </summary>
    public class EntityContext:IEntityContext
    {
        #region Fields

        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly MappingContext _mappingContext;
        private readonly INodeConverter _nodeConverter;

        #endregion

        #region Constructors

        internal EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            MappingContext mappingContext, 
            IEntityStore entityStore, 
            IEntitySource entitySource)
        {
            LogTo.Info("Creating entity context");
            _factory=factory;
            _entityStore=entityStore;
            _entitySource=entitySource;
            _nodeConverter=new NodeConverter(this,entityStore);
            _mappings=mappings;
            _mappingContext = mappingContext;
            Cache = new DictionaryCache();
            factory.SatisfyImports(_nodeConverter);
        }
        #endregion

        #region Properties
        // TODO: Consider hiding this Cache member.

        /// <summary>Gets the cache of this entity contxt.</summary>
        public ICache Cache { get; set; }

        // TODO: Consider hiding this Store member.

        /// <summary>Gets the underlying in-memory store.</summary>
        public IEntityStore Store { get { return _entityStore; } }

        /// <summary>Gets a value indicating whether the underlying store has any changes.</summary>
        public bool HasChanges
        {
            get
            {
                return Store.Changes.Any;
            }
        }
        #endregion

        #region Public methods
        
        /// <inheritdoc />
        public IQueryable<Entity> AsQueryable()
        {
            return new EntityQueryable<Entity>(this,_entitySource,_mappings);
        }

        /// <inheritdoc />
        public IQueryable<T> AsQueryable<T>() where T:class,IEntity
        {
            return new EntityQueryable<T>(this,_entitySource,_mappings);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public T Load<T>(EntityId entityId) where T : class,IEntity
        {
            return Load<T>(entityId,true);
        }

        /// <summary>Loads an entity from the underlying data source.</summary>
        /// <param name="entityId">IRI of the entity to be loaded.</param>
        /// <param name="checkIfExist">Determines whether to check if entity does exist or not.</param>
        /// <returns>Loaded entity.</returns>
        [return: AllowNull]
        public T Load<T>(EntityId entityId,bool checkIfExist) where T:class,IEntity
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

        /// <inheritdoc />
        public T Create<T>(EntityId entityId) where T : class,IEntity
        {
            if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
            {
                return (T)(IEntity)Create(entityId);
            }

            var entity=Create(entityId);

            var entityMapping=_mappings.MappingFor<T>();
            if (entityMapping==null)
            {
                throw new UnMappedTypeException(typeof(T));
            }

            var classMappings = entityMapping.Classes;
            foreach (var classMapping in classMappings)
            {
                var typedEntity = AsTypedEntity(entity,classMapping);
                typedEntity.Types=new[] { new EntityId(classMapping.Uri) };
            }

            return EntityAs<T>(entity);
        }

        /// <inheritdoc />
        public Entity Create(EntityId entityId)
        {
            LogTo.Debug("Creating entity {0}", entityId);
            return Create(entityId,true);
        }

        /// <inheritdoc />
        public void Commit()
        {
            var changes=_entityStore.Changes;
            _entitySource.ApplyChanges(changes);
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId)
        {
            _entityStore.Delete(entityId);
        }

        void IDisposable.Dispose()
        {
            // todo: implement
        }

        #endregion

        #region Non-public methods
        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
        internal void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}",entity.Id);
            _entitySource.LoadEntity(Store,entity.Id);
        }

        /// <summary>Transforms given entity into a strongly typed interface.</summary>
        /// <typeparam name="T">Type of the interface to transform given entity to.</typeparam>
        /// <param name="entity">Entity to be transformed.</param>
        /// <returns>Passed entity beeing a given interface.</returns>
        internal T EntityAs<T>(Entity entity) where T:class,IEntity
        {
            if (typeof(T) == typeof(IEntity))
            {
                return (T)(IEntity)entity;
            }

            LogTo.Trace("Wrapping entity {0} as {1}", entity.Id, typeof(T));
            return EntityAs<T>(entity,_mappings.MappingFor<T>());
        }

        [Cache]
        [return: AllowNull]
        private Entity Load(EntityId entityId, bool checkIfExist = true)
        {
            LogTo.Debug("Loading entity {0}", entityId);

            if ((entityId is BlankId) || (!checkIfExist) || (_entitySource.EntityExist(entityId)))
            {
                return Create(entityId, false);
            }

            return null;
        }

        private Entity Create(EntityId entityId, bool entityExists)
        {
            var entity=new Entity(entityId,this,entityExists);

            foreach (var ontology in _mappingContext.OntologyProvider.Ontologies)
            {
                var ontologyAccessor=new OntologyAccessor(Store,entity,ontology,_nodeConverter);
                _factory.SatisfyImports(ontologyAccessor);
                entity[ontology.Prefix] = ontologyAccessor;
            }

            return entity;
        }

        /// <summary>
        /// Creates an instance of ITypedEntity with custom mapping 
        /// to place rdf:type triple in correct named graph as declared by the parent mapping
        /// </summary>
        private ITypedEntity AsTypedEntity(Entity entity, IClassMapping classMapping)
        {
            var map = new TypeEntityMap(classMapping.GraphSelector.SelectGraph(entity.Id));
            return EntityAs<ITypedEntity>(entity, map.CreateMapping(_mappingContext));
        }

        private T EntityAs<T>(Entity entity, IEntityMapping mapping) where T : class,IEntity
        {
            var proxy = new EntityProxy(Store, entity, mapping, _nodeConverter);
            _factory.SatisfyImports(proxy);
            return proxy.ActLike<T>();
        }
        #endregion
    }
}