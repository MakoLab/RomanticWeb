using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.Updates;

namespace RomanticWeb
{
    /// <summary>
    /// Creates a new instance of <see cref="EntityContext"/>
    /// </summary>
    [NullGuard(ValidationFlags.All)]
    internal class EntityContext : IEntityContext
    {
        #region Fields
        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly IBaseUriSelectionPolicy _baseUriSelector;
        private readonly IResultTransformerCatalog _transformerCatalog;
        private readonly IRdfTypeCache _typeCache;
        private readonly IBlankNodeIdGenerator _blankIdGenerator;
        private readonly IEntityCaster _caster;
        private readonly IDatasetChanges _changeTracker;
        private readonly IEntityMapping _typedEntityMapping;
        private readonly IPropertyMapping _typesPropertyMapping;

        private bool _disposed;
        #endregion

        #region Constructors

        public EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            IEntityStore entityStore,
            IEntitySource entitySource,
            [AllowNull] IBaseUriSelectionPolicy baseUriSelector,
            IRdfTypeCache typeCache,
            IBlankNodeIdGenerator blankIdGenerator,
            IResultTransformerCatalog transformerCatalog, 
            IEntityCaster caster, 
            IDatasetChangesTracker changeTracker)
            : this(changeTracker)
        {
            _factory = factory;
            _entityStore = entityStore;
            _entitySource = entitySource;
            _baseUriSelector = baseUriSelector;
            _mappings = mappings;
            _typeCache = typeCache;
            _blankIdGenerator = blankIdGenerator;
            _transformerCatalog = transformerCatalog;
            _caster = caster;
            _typedEntityMapping = _mappings.MappingFor<ITypedEntity>();
            _typesPropertyMapping = _typedEntityMapping.PropertyFor("Types");

            if (_baseUriSelector == null)
            {
                LogTo.Warn("No Base URI Selection Policy. It will not be possible to use relative URIs");
            }
        }

        public EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            IEntityStore entityStore,
            IEntitySource entitySource,
            IRdfTypeCache typeCache,
            IBlankNodeIdGenerator blankIdGenerator,
            IResultTransformerCatalog transformerCatalog,
            IEntityCaster caster,
            IDatasetChangesTracker changeTracker)
            : this(
                factory,
                mappings,
                entityStore,
                entitySource,
                null,
                typeCache,
                blankIdGenerator,
                transformerCatalog, 
                caster, 
                changeTracker)
        {
        }

        private EntityContext(IDatasetChangesTracker changeTracker)
        {
            _changeTracker = changeTracker;
            LogTo.Info("Creating entity context");
            EntityCache = new InMemoryEntityCache();
        }
        #endregion

        public event Action Disposed;
        
        #region Properties

        /// <summary>Gets the underlying in-memory store.</summary>
        public IEntityStore Store { get { return _entityStore; } }

        /// <summary>Gets a value indicating whether the underlying store has any changes.</summary>
        public bool HasChanges { get { return _changeTracker.HasChanges; } }

        /// <inheritdoc />
        public IBlankNodeIdGenerator BlankIdGenerator { get { return _blankIdGenerator; } }

        /// <inheritdoc />
        public IOntologyProvider Ontologies { get { return _factory.Ontologies; } }

        /// <inheritdoc />
        public IMappingsRepository Mappings { get { return _mappings; } }

        /// <inheritdoc />
        [AllowNull]
        public IBaseUriSelectionPolicy BaseUriSelector { get { return _baseUriSelector; } }

        public IDatasetChanges Changes
        {
            get
            {
                return _changeTracker;
            }
        }

        internal IEntityCache EntityCache { get; private set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IQueryable<IEntity> AsQueryable()
        {
            return new EntityQueryable<IEntity>(this, _entitySource, _entityStore);
        }

        /// <inheritdoc />
        public IQueryable<T> AsQueryable<T>() where T : class, IEntity
        {
            return new EntityQueryable<T>(this, _entitySource, _entityStore);
        }

        /// <summary>Loads an entity from the underlying data source.</summary>
        /// <param name="entityId">IRI of the entity to be loaded.</param>
        /// <returns>Loaded entity.</returns>
        public T Load<T>(EntityId entityId) where T : class, IEntity
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Loading entity {0}", entityId);
            return EntityAs<T>(LoadInternal(entityId));
        }

        /// <inheritdoc />
        public T Create<T>(EntityId entityId) where T : class, IEntity
        {
            if (typeof(T) == typeof(Entity))
            {
                return (T)(IEntity)CreateInternal(entityId, true);
            }

            var entity = CreateInternal(entityId, true);
            return EntityAs<T>(entity);
        }

        /// <inheritdoc />
        public void Commit()
        {
            LogTo.Info("Committing changes to triple store");
            _entitySource.Commit(Changes);
            _entityStore.ResetState();
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId)
        {
            Delete(entityId, DeleteBehaviour.Default);
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId, DeleteBehaviour deleteBehaviour)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Deleting entity {0}", entityId);
            _entityStore.Delete(entityId, deleteBehaviour);
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _entityStore.Dispose();

            _disposed = true;

            if (Disposed != null)
            {
                Disposed();
            }
        }

        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
        public void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}", entity.Id);
            _entityStore.AssertEntity(entity.Id, _entitySource.LoadEntity(entity.Id));
        }

        /// <inheritdoc />
        public T EntityAs<T>(IEntity entity) where T : class, IEntity
        {
            var rootEntity = (Entity)entity;
            rootEntity.EnsureIsInitialized();
            Uri graphName = (_factory.NamedGraphSelector != null ? _factory.NamedGraphSelector.SelectGraph(rootEntity.Id, _typedEntityMapping, _typesPropertyMapping) : null);
            IEnumerable<Uri> types = _entityStore.GetObjectsForPredicate(rootEntity.Id, Vocabularies.Rdf.type, graphName).Select(item => item.Uri);
            var entityTypes = _typeCache.GetMostDerivedMappedTypes(types, typeof(T));
            return _caster.EntityAs<T>(rootEntity, entityTypes.ToArray());
        }

        /// <inheritdoc />
        public bool Exists(EntityId entityId)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            return _entitySource.EntityExist(entityId);
        }

        public void Rollback()
        {
            _entityStore.Rollback();
        }

        #endregion

        #region Non-public methods
        private Entity LoadInternal(EntityId entityId)
        {
            return CreateInternal(entityId, entityId is BlankId);
        }

        private Entity CreateInternal(EntityId entityId, bool markAsInitialized)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            if (!EntityCache.HasEntity(entityId))
            {
                LogTo.Info("Creating entity {0}", entityId);
                var entity = new Entity(entityId, this, _factory.FallbackNodeConverter, _transformerCatalog);

                if (markAsInitialized)
                {
                    entity.MarkAsInitialized();
                }

                EntityCache.Add(entity);
            }

            return EntityCache.Get(entityId);
        }

        private EntityId EnsureAbsoluteEntityId(EntityId entityId)
        {
            if (!entityId.Uri.IsAbsoluteUri)
            {
                entityId = entityId.MakeAbsolute(_baseUriSelector.SelectBaseUri(entityId));
            }

            return entityId;
        }
        #endregion
    }
}