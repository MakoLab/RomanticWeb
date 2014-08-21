using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;
using RomanticWeb.Vocabularies;

namespace RomanticWeb
{
    /// <summary>
    /// Creates a new instance of <see cref="EntityContext"/>
    /// </summary>
    [NullGuard(ValidationFlags.All)]
    internal class EntityContext : IEntityContext
    {
        #region Fields
        private static readonly EntityMapping EntityMapping = new EntityMapping(typeof(IEntity));
        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly MappingContext _mappingContext;
        private readonly IBaseUriSelectionPolicy _baseUriSelector;
        private readonly IResultTransformerCatalog _transformerCatalog;
        private readonly IRdfTypeCache _typeCache;
        private readonly IBlankNodeIdGenerator _blankIdGenerator;

        #endregion

        #region Constructors

        public EntityContext(
            IEntityContextFactory factory,
            IMappingsRepository mappings,
            MappingContext mappingContext,
            IEntityStore entityStore,
            IEntitySource entitySource,
            [AllowNull] IBaseUriSelectionPolicy baseUriSelector,
            INamedGraphSelector namedGraphSelector,
            IRdfTypeCache typeCache,
            IBlankNodeIdGenerator blankIdGenerator,
            IResultTransformerCatalog transformerCatalog)
        {
            LogTo.Info("Creating entity context");
            if (baseUriSelector == null)
            {
                LogTo.Warn("No Base URI Selection Policy. It will not be possible to use relative URIs");
            }

            _baseUriSelector = baseUriSelector;
            _factory = factory;
            _entityStore = entityStore;
            _entitySource = entitySource;
            _mappings = mappings;
            _mappingContext = mappingContext;
            GraphSelector = namedGraphSelector;
            _typeCache = typeCache;
            _blankIdGenerator = blankIdGenerator;
            _transformerCatalog = transformerCatalog;

            EntityCache = new InMemoryEntityCache();
        }

        #endregion

        #region Properties

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
        [AllowNull]
        public IBaseUriSelectionPolicy BaseUriSelector { get { return _baseUriSelector; } }

        internal IEntityCache EntityCache { get; private set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IQueryable<IEntity> AsQueryable()
        {
            return new EntityQueryable<IEntity>(this, _entitySource, _mappings, _baseUriSelector);
        }

        /// <inheritdoc />
        public IQueryable<T> AsQueryable<T>() where T : class, IEntity
        {
            return new EntityQueryable<T>(this, _entitySource, _mappings, _baseUriSelector);
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
            if ((typeof(T) == typeof(IEntity)) || (typeof(T) == typeof(Entity)))
            {
                return (T)(IEntity)CreateInternal(entityId, true);
            }

            var entity = CreateInternal(entityId, true);
            AssertEntityTypes<T>(entity);
            return EntityAs<T>(entity);
        }

        /// <inheritdoc />
        public void Commit()
        {
            LogTo.Info("Committing changes to triple store");
            _entitySource.ApplyChanges(_entityStore.Changes);
            _entityStore.ResetState();
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId)
        {
            Delete(entityId, DeleteBehaviours.Default);
        }

        /// <inheritdoc />
        public void Delete(EntityId entityId, DeleteBehaviours deleteBehaviour)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            LogTo.Info("Deleting entity {0}", entityId);
            _entityStore.Delete(entityId, deleteBehaviour);
        }

        void IDisposable.Dispose()
        {
            // todo: implement
        }

        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
        public void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}", entity.Id);
            _entitySource.LoadEntity(Store, entity.Id);
        }

        /// <inheritdoc />
        public T EntityAs<T>(IEntity entity) where T : class, IEntity
        {
            var rootEntity = (Entity)entity;
            rootEntity.EnsureIsInitialized();
            IEnumerable<Uri> entityTypeUris = _entityStore.GetObjectsForPredicate(rootEntity.Id, Rdf.type, GraphSelector.SelectGraph(rootEntity.Id, null, null)).Select(item => item.Uri);
            var entityTypes = _typeCache.GetMostDerivedMappedTypes(entityTypeUris, typeof(T));
            return EntityAs(rootEntity, typeof(T), entityTypes.ToArray());
        }

        /// <inheritdoc />
        public bool Exists(EntityId entityId)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
            return _entitySource.EntityExist(entityId);
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
                var entity = new Entity(entityId, this);

                foreach (var ontology in _mappingContext.OntologyProvider.Ontologies)
                {
                    var ontologyAccessor = new OntologyAccessor(entity, ontology, _factory.Services.GetService<FallbackNodeConverter>(), TransformerCatalog);
                    entity[ontology.Prefix] = ontologyAccessor;
                }

                if (markAsInitialized)
                {
                    entity.MarkAsInitialized();
                }

                EntityCache.Add(entity);
            }

            return EntityCache.Get(entityId);
        }

        private void AssertEntityTypes<T>(Entity entity)
        {
            var entityMapping = _mappings.MappingFor<T>();
            if (entityMapping == null)
            {
                throw new UnMappedTypeException(typeof(T));
            }

            AssertEntityTypes(entity, entityMapping);
        }

        private void AssertEntityTypes(Entity entity, IEntityMapping entityMapping)
        {
            var graph = GraphSelector.SelectGraph(entity.Id, entityMapping, null);
            var currentTypes = Store.GetObjectsForPredicate(entity.Id, Rdf.type, graph);

            var rdfTypes = currentTypes.Union(entityMapping.Classes.Select(c => Node.ForUri(c.Uri)));

            Store.ReplacePredicateValues(entity.Id, Node.ForUri(Rdf.type), rdfTypes.ToList, graph);
        }

        private EntityId EnsureAbsoluteEntityId(EntityId entityId)
        {
            if (!entityId.Uri.IsAbsoluteUri)
            {
                entityId = entityId.MakeAbsolute(_baseUriSelector.SelectBaseUri(entityId));
            }

            return entityId;
        }

        private dynamic EntityAs(Entity entity, Type requested, Type[] types)
        {
            IEntityMapping mapping;
            if (types.Length == 1)
            {
                mapping = GetMapping(types[0]);
            }
            else if (types.Length == 0)
            {
                types = new[] { requested };
                mapping = GetMapping(requested);
            }
            else
            {
                mapping = new MultiMapping(types.Select(GetMapping).ToArray());
            }

            return EntityAs(entity, mapping, types);
        }

        private dynamic EntityAs(Entity entity, IEntityMapping mapping, Type[] types)
        {
            AssertEntityTypes(entity, mapping);

            var proxy = new EntityProxy(entity, mapping, TransformerCatalog);
            return Impromptu.DynamicActLike(proxy, types);
        }

        private IEntityMapping GetMapping(Type type)
        {
            if (type == typeof(IEntity))
            {
                return EntityMapping;
            }

            var mapping = _mappings.MappingFor(type);
            if (mapping == null)
            {
                throw new UnMappedTypeException(type);
            }

            return mapping;
        }
        #endregion
    }
}