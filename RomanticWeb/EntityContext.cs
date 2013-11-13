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
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public class EntityContext:IEntityContext
    {
        #region Fields

        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly IOntologyProvider _ontologyProvider;
        private readonly INodeConverter _nodeConverter;

        #endregion

        #region Constructors

        /// <summary>Creates an instance of an entity context with given mappings and entity source.</summary>
        /// <param name="factory">Factory, which created this entity context</param>
        /// <param name="mappings">Information defining strongly typed interface mappings.</param>
        /// <param name="entitySource">Physical entity data source.</param>
        [Obsolete]
        internal EntityContext(IEntityContextFactory factory,IMappingsRepository mappings,IEntitySource entitySource)
            :this(factory,mappings,new DefaultOntologiesProvider(),new EntityStore(),entitySource)
        {
        }

        internal EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            IOntologyProvider ontologyProvider, 
            IEntityStore entityStore, 
            IEntitySource entitySource)
        {
            LogTo.Info("Creating entity context");
            _factory=factory;
            _entityStore=entityStore;
            _entitySource=entitySource;
            _nodeConverter=new NodeConverter(this,entityStore);
            _mappings=mappings;
            _ontologyProvider=ontologyProvider;
            Cache = new DictionaryCache();
            factory.SatisfyImports(_nodeConverter);
        }
        #endregion

        #region Properties

        public ICache Cache { get; set; }

        public IEntityStore Store
        {
            get
            {
                return _entityStore;
            }
        }

        public bool HasChanges
        {
            get
            {
                return Store.Changes.Any;
            }
        }

        #endregion

        #region Public methods
        /// <summary>Converts this context into a LINQ queryable data source.</summary>
        /// <returns>A LINQ querable data source.</returns>
        public IQueryable<Entity> AsQueryable()
        {
            return new EntityQueryable<Entity>(this,_mappings,_ontologyProvider);
        }

        /// <summary>Converts this context into a LINQ queryable data source of entities of given type.</summary>
        /// <typeparam name="T">Type of entities to work with.</typeparam>
        /// <returns>A LIQN queryable data source of entities of given type.</returns>
        public IQueryable<T> AsQueryable<T>() where T:class,IEntity
        {
            return new EntityQueryable<T>(this,_mappings,_ontologyProvider);
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
            if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
            {
                return (T)(IEntity)Create(entityId);
            }

            var entity=Create(entityId);

            var classMapping=_mappings.MappingFor<T>().Class;
            if (classMapping!=null)
            {
                var typedEntity = AsTypedEntity(entity,classMapping);
                typedEntity.Types=new[] { new EntityId(classMapping.Uri) };
            }

            return EntityAs<T>(entity);
        }

        public Entity Create(EntityId entityId)
        {
            LogTo.Debug("Creating entity {0}", entityId);
            return Create(entityId,true);
        }

        public void Commit()
        {
            var changes=_entityStore.Changes;
            _entitySource.ApplyChanges(changes);
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

        private Entity Create(EntityId entityId, bool entityExists)
        {
            var entity=new Entity(entityId,this,entityExists);

            foreach (var ontology in _ontologyProvider.Ontologies)
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
            return EntityAs<ITypedEntity>(entity,map.CreateMapping(_ontologyProvider));
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