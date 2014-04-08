using System;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using MethodCache.Attributes;
using NullGuard;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Creates a new instance of <see cref="EntityContext"/>
    /// </summary>
    public class EntityContext:IEntityContext
    {
        #region Fields
        private static readonly EntityMapping EntityMapping=new EntityMapping(typeof(IEntity));
        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly MappingContext _mappingContext;
        private readonly IBaseUriSelectionPolicy _baseUriSelector;
        private readonly IResultTransformerCatalog _transformerCatalog;
        private readonly IRdfTypeCache _typeCache;

        private IBlankNodeIdGenerator _blankIdGenerator=new DefaultBlankNodeIdGenerator();
        #endregion

        #region Constructors
        internal EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            MappingContext mappingContext,
            IEntityStore entityStore,
            IEntitySource entitySource,
            IBaseUriSelectionPolicy baseUriSelector,
            INamedGraphSelector namedGraphSelector,
            IRdfTypeCache typeCache)
        {
            LogTo.Info("Creating entity context");
            _factory=factory;
            _entityStore=entityStore;
            _entitySource=entitySource;
            _baseUriSelector=baseUriSelector;
            _mappings=mappings;
            _mappingContext=mappingContext;
            Cache=new DictionaryCache();
            GraphSelector=namedGraphSelector;
            _typeCache=typeCache;
            _transformerCatalog=new ResultTransformerCatalog();
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
        public bool HasChanges { get { return Store.Changes.Any; } }

        /// <inheritdoc />
        public IBlankNodeIdGenerator BlankIdGenerator { get { return _blankIdGenerator; } }

        /// <inheritdoc />
        public IOntologyProvider Ontologies { get { return _factory.Ontologies; } }

        /// <inheritdoc />
        public INamedGraphSelector GraphSelector { get; private set; }

        /// <inheritdoc />
        public IResultTransformerCatalog TransformerCatalog
        {
            get
            {
                return _transformerCatalog;
            }
        }

        /// <inheritdoc />
        public IMappingsRepository Mappings { get { return _mappings; } }

        /// <inheritdoc />
        public IBaseUriSelectionPolicy BaseUriSelector { get { return _baseUriSelector; } }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IQueryable<IEntity> AsQueryable()
        {
            return new EntityQueryable<IEntity>(this,_entitySource,_mappings,_baseUriSelector);
        }

        /// <inheritdoc />
        public IQueryable<T> AsQueryable<T>() where T:class,IEntity
        {
            return new EntityQueryable<T>(this,_entitySource,_mappings,_baseUriSelector);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public T Load<T>(EntityId entityId) where T:class,IEntity
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
            IEntity entity=LoadInternal(entityId,checkIfExist);
            if (entity==null)
            {
                return null;
            }

            return EntityAs<T>(entity);
        }

        /// <inheritdoc />
        public T Create<T>(EntityId entityId) where T:class,IEntity
        {
            if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
            {
                return (T)(IEntity)Create(entityId);
            }

            return EntityAs<T>(BuildEntityTypes<T>(Create(entityId)));
        }

        /// <inheritdoc />
        public Entity Create(EntityId entityId)
        {
            entityId=EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Creating entity {0}",entityId);
            return Create(entityId,true);
        }

        /// <inheritdoc />
        public void Commit()
        {
            LogTo.Info("Committing changes to triple store");
            var changes=_entityStore.Changes;
            _entitySource.ApplyChanges(changes);
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId)
        {
            entityId=EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Deleting entity {0}",entityId);
            _entityStore.Delete(entityId);
        }

        void IDisposable.Dispose()
        {
            // todo: implement
        }

        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
        public void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}",entity.Id);
            _entitySource.LoadEntity(Store,entity.Id);
        }

        /// <summary>Transforms given entity into a strongly typed interface.</summary>
        /// <typeparam name="T">Type of the interface to transform given entity to.</typeparam>
        /// <param name="entity">Entity to be transformed.</param>
        /// <returns>Passed entity beeing a given interface.</returns>
        public T EntityAs<T>(IEntity entity) where T:class,IEntity
        {
            var entityTypes=_typeCache.GetMostDerivedMappedTypes(entity,typeof(T));
            return EntityAs((Entity)entity,typeof(T),entityTypes.ToArray());
        }

        #endregion

        #region Non-public methods
        [Cache]
        [return: AllowNull]
        private Entity LoadInternal(EntityId entityId,bool checkIfExist)
        {
            entityId=EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Loading entity {0}",entityId);

            if ((entityId is BlankId)||(!checkIfExist)||(_entitySource.EntityExist(entityId)))
            {
                return Create(entityId,false);
            }

            return null;
        }

        private Entity Create(EntityId entityId,bool entityExists)
        {
            var entity=new Entity(entityId,this,entityExists);

            foreach (var ontology in _mappingContext.OntologyProvider.Ontologies)
            {
                var ontologyAccessor=new OntologyAccessor(entity,ontology,TransformerCatalog);
                entity[ontology.Prefix]=ontologyAccessor;
            }

            return entity;
        }

        private Entity BuildEntityTypes<T>(Entity entity)
        {
            var entityMapping=_mappings.MappingFor<T>();
            if (entityMapping==null)
            {
                throw new UnMappedTypeException(typeof(T));
            }

            var types=entity.AsEntity<ITypedEntity>().Types;
            foreach (var classMapping in entityMapping.Classes)
            {
                classMapping.AppendTo(types);
            }

            return entity;
        }

        private EntityId EnsureAbsoluteEntityId(EntityId entityId)
        {
            if (!entityId.Uri.IsAbsoluteUri)
            {
                entityId=entityId.MakeAbsolute(_baseUriSelector.SelectBaseUri(entityId));
            }

            return entityId;
        }

        private dynamic EntityAs(Entity entity,Type requested,Type[] types)
        {
            IEntityMapping mapping;
            if (types.Length==1)
            {
                mapping=GetMapping(types[0]);
            }
            else if (types.Length==0)
            {
                types=new[] { requested };
                mapping=GetMapping(requested);
            }
            else
            {
                mapping=new MultiMapping(types.Select(GetMapping).ToArray());                
            }

            return EntityAs(entity,mapping,types);
        }

        private dynamic EntityAs(Entity entity,IEntityMapping mapping,params Type[] types)
        {
            var proxy=new EntityProxy(entity,mapping,TransformerCatalog);
            return Impromptu.DynamicActLike(proxy,types);
        }

        private IEntityMapping GetMapping(Type type)
        {
            if (type==typeof(IEntity))
            {
                return EntityMapping;
            }

            var mapping=_mappings.MappingFor(type);
            if (mapping==null)
            {
                throw new UnMappedTypeException(type);
            }

            return mapping;
        }
        #endregion
    }
}