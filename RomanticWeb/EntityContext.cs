using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using ImpromptuInterface;
using MethodCache.Attributes;
using NullGuard;
using RomanticWeb.Converters;
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
        private readonly IEntityContextFactory _factory;
        private readonly IEntityStore _entityStore;
        private readonly IEntitySource _entitySource;
        private readonly IMappingsRepository _mappings;
        private readonly MappingContext _mappingContext;
        private readonly INodeConverter _nodeConverter;
        private readonly IDictionary<string,Type> _classInterfacesMap;
        private readonly IList<string> _missingClassInterfacesMap;
        private readonly IBaseUriSelectionPolicy _baseUriSelector;

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
            INamedGraphSelector namedGraphSelector)
        {
            LogTo.Info("Creating entity context");
            _factory=factory;
            _entityStore=entityStore;
            _entitySource=entitySource;
            _baseUriSelector=baseUriSelector;
            _nodeConverter=new NodeConverter(this,factory.Converters);
            _mappings=mappings;
            _mappingContext=mappingContext;
            Cache=new DictionaryCache();
            _classInterfacesMap=new Dictionary<string,Type>();
            _missingClassInterfacesMap=new List<string>();
            GraphSelector=namedGraphSelector;
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

        public IOntologyProvider Ontologies { get { return _factory.Ontologies; } }

        public INodeConverter NodeConverter { get { return _nodeConverter; } }

        public INamedGraphSelector GraphSelector { get; private set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IQueryable<IEntity> AsQueryable()
        {
            return new EntityQueryable<IEntity>(this,_entitySource,_mappings,_baseUriSelector);
        }

        /// <inheritdoc />e
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
            var entity=Load(entityId,checkIfExist);

            if (entity==null)
            {
                return null;
            }

            T result;
            Type entityType=GetEntityType(entity);
            if ((entityType!=null)&&(typeof(T).IsAssignableFrom(entityType)))
            {
                result=(T)GetType().GetMethod("EntityAs",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new[] { typeof(Entity) },null)
                    .MakeGenericMethod(entityType)
                    .Invoke(this,new object[] { entity });
            }
            else if ((typeof(T)==typeof(IEntity))||(typeof(T)==typeof(Entity)))
            {
                result=(T)(IEntity)entity;
            }
            else
            {
                result=EntityAs<T>(entity);
            }

            return result;
        }

        /// <inheritdoc />
        public T Create<T>(EntityId entityId) where T:class,IEntity
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

            var classMappings=entityMapping.Classes;
            foreach (var classMapping in classMappings)
            {
                var typedEntity=entity.AsEntity<ITypedEntity>();
                typedEntity.Types=new[] { new EntityId(classMapping.Uri) };
            }

            return EntityAs<T>(entity);
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
            LogTo.Info("Deleting entity {0}", entityId);
            _entityStore.Delete(entityId);
        }

        void IDisposable.Dispose()
        {
            // todo: implement
        }

        #region Non-public methods
        /// <summary>Initializes given entity with data.</summary>
        /// <param name="entity">Entity to be initialized</param>
        public void InitializeEnitity(IEntity entity)
        {
            LogTo.Debug("Initializing entity {0}", entity.Id);
            _entitySource.LoadEntity(Store, entity.Id);
        }

        /// <summary>Transforms given entity into a strongly typed interface.</summary>
        /// <typeparam name="T">Type of the interface to transform given entity to.</typeparam>
        /// <param name="entity">Entity to be transformed.</param>
        /// <returns>Passed entity beeing a given interface.</returns>
        public T EntityAs<T>(IEntity entity) where T : class,IEntity
        {
            if (typeof(T) == typeof(IEntity))
            {
                return (T)entity;
            }

            LogTo.Trace("Wrapping entity {0} as {1}", entity.Id, typeof(T));
            return EntityAs<T>((Entity)entity, _mappings.MappingFor<T>());
        }
        #endregion

        [Cache]
        [return: AllowNull]
        private Entity Load(EntityId entityId,bool checkIfExist=true)
        {
            entityId = EnsureAbsoluteEntityId(entityId);
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
                var ontologyAccessor=new OntologyAccessor(entity,ontology,_factory.TransformerCatalog,_factory.Converters);
                entity[ontology.Prefix]=ontologyAccessor;
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

        private T EntityAs<T>(Entity entity,IEntityMapping mapping) where T:class,IEntity
        {
            if (mapping==null)
            {
                throw new ArgumentNullException("mapping", string.Format("Mappings not found for type {0}",typeof(T)));
            }

            var proxy=new EntityProxy(entity,mapping,_factory.TransformerCatalog);
            return proxy.ActLike<T>();
        }
       
        private IList<Type> GetEntityTypes(Entity entity)
        {
            IEnumerable<EntityId> entityClasses=entity.AsEntity<ITypedEntity>().Types??new EntityId[0];
            IList<Type> entityTypes=new List<Type>();
            if (entityClasses.Any())
            {
                foreach (EntityId classId in entityClasses)
                {
                    if (!_missingClassInterfacesMap.Contains(classId.Uri.AbsoluteUri))
                    {
                        Type type=null;
                        if (_classInterfacesMap.ContainsKey(classId.Uri.AbsoluteUri))
                        {
                            type=_classInterfacesMap[classId.Uri.AbsoluteUri];
                        }
                        else
                        {
                            type=_mappings.MappingFor(classId.Uri);
                            if ((type!=null)&&(type.IsInterface))
                            {
                                _classInterfacesMap[classId.Uri.AbsoluteUri]=type;
                            }
                            else
                            {
                                _missingClassInterfacesMap.Add(classId.Uri.AbsoluteUri);
                            }
                        }

                        if (type!=null)
                        {
                            entityTypes.Add(type);
                        }
                    }
                }
            }

            return entityTypes;
        }

        private Type GetEntityType(Entity entity)
        {
            Type result=null;
            IList<Type> entityTypes=GetEntityTypes(entity);
            if (entityTypes.Count>0)
            {
                result=entityTypes[0];
                foreach (Type entityType in entityTypes.Skip(1))
                {
                    if (entityType.GetInterfaces().Contains(result))
                    {
                        result=entityType;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}