using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public class EntityContextFactory:IEntityContextFactory
    {
        #region Fields
        private readonly MappingBuilder _mappingBuilder = new MappingBuilder();
        private readonly CompositionContainer _container;
        private bool _isInitialized;

        [ImportMany(typeof(IOntologyProvider),AllowRecomposition=true)]
        private IEnumerable<IOntologyProvider> _importedOntologies=new IOntologyProvider[0];

        [ImportMany(typeof(IMappingsRepository),AllowRecomposition=true)]
        private IEnumerable<IMappingsRepository> _importedMappings=new IMappingsRepository[0];

        private Func<IEntitySource> _entitySourceFactory;
        private MappingContext _mappingContext;
        private Func<IEntityStore> _entityStoreFactory=() => new EntityStore();
        private GraphSelectionStrategyBase _defaultGraphSelector=new DefaultGraphSelector();
        private IMappingsRepository _actualMappingsRepository;
        private IOntologyProvider _actualOntologyProvider;
        private IBaseUriSelectionPolicy _baseUriSelector;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
        {
            var catalog=new DirectoryCatalog(AppDomain.CurrentDomain.GetPrimaryAssemblyPath());
            _container=new CompositionContainer(catalog, true);
            catalog.Changed+=CatalogChanged;
            WithMappings(DefaultMappings);
            LogTo.Info("Created entity context factory");
        }
        #endregion

        #region Properties
        /// <summary>Gets the ontology provider.</summary>
        public IOntologyProvider Ontologies
        {
            get
            {
                EnsureInitialized();
                return _actualOntologyProvider;
            }
        }

        /// <summary>Gets the mappings.</summary>
        public IMappingsRepository Mappings
        {
            get
            {
                EnsureInitialized();
                return _actualMappingsRepository;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a new instance of entity context
        /// </summary>
        public IEntityContext CreateContext()
        {
            LogTo.Debug("Creating entity context");

            EnsureComplete();
            EnsureInitialized();
            _mappingContext=new MappingContext(_actualOntologyProvider,_defaultGraphSelector);

            return new EntityContext(this,Mappings,_mappingContext,_entityStoreFactory(),_entitySourceFactory(),_baseUriSelector);
        }

        /// <summary>Satisfies imports for given object.</summary>
        /// <param name="obj">Target object to be satisfied.</param>
        public void SatisfyImports(object obj)
        {
            _container.ComposeParts(obj);
        }

        /// <summary>Includes a given <see cref="IEntitySource" /> in context that will be created.</summary>
        /// <param name="entitySource">Target entity source.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _entitySourceFactory=entitySource;
            return this;
        }

        /// <summary>Includes a given <see cref="IOntologyProvider" /> in context that will be created.</summary>
        /// <param name="ontologyProvider">Target ontology provider.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithOntology(IOntologyProvider ontologyProvider)
        {
            _container.ComposeExportedValue(ontologyProvider);
            return this;
        }

        /// <summary>Includes a given <see cref="IEntityStore" /> in context that will be created.</summary>
        /// <param name="entityStoreFactory">Target entity store.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory)
        {
            _entityStoreFactory=entityStoreFactory;
            return this;
        }

        /// <summary>Exposes the method to register mapping repositories.</summary>
        /// <param name="buildMappings">Delegate method to be used for building mappings.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithMappings(Action<MappingBuilder> buildMappings)
        {
            var repositories=_mappingBuilder.BuildMappings(buildMappings);

            foreach (var mappingsRepository in repositories)
            {
                _container.ComposeExportedValue(mappingsRepository);
            }

            return this;
        }

        /// <summary>Exposes the method to register a default graph selector.</summary>
        /// <param name="graphSelector">Delegate method to be used for selecting graph names.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithDefaultGraphSelector(GraphSelectionStrategyBase graphSelector)
        {
            _defaultGraphSelector=graphSelector;
            return this;
        }

        /// <summary>
        /// Exposes a method to define how base <see cref="Uri"/>s are selected for relavitve <see cref="EntityId"/>s
        /// </summary>
        public EntityContextFactory WithBaseUri(Action<BaseUriSelectorBuilder> setupPolicy)
        {
            var builder=new BaseUriSelectorBuilder();
            setupPolicy(builder);
            _baseUriSelector=builder.Build();
            return this;
        }
        #endregion

        #region Non-public methods
        private static void DefaultMappings(MappingBuilder mappings)
        {
            mappings.Fluent.FromAssemblyOf<ITypedEntity>();
            mappings.Attributes.FromAssemblyOf<ITypedEntity>();
        }

        private void EnsureInitialized()
        {
            LogTo.Info("Initializing entity context factory");
            if (_isInitialized)
            {
                return;
            }

            var batch=new CompositionBatch();
            batch.AddPart(this);

            _container.Compose(batch);
            EnsureOntologyProvider();
            EnsureMappingsRepository();
            EnsureMappingsRebuilt();

            _isInitialized=true;
        }

        private void CatalogChanged(object sender,ComposablePartCatalogChangeEventArgs changeEventArgs)
        {
            LogTo.Info("MEF catalog has changed");
            bool shouldRebuildMappings=false;
            
            if (changeEventArgs.AddedDefinitions.Any(def => def.Exports<IMappingsRepository>()))
            {
                LogTo.Info("Refreshing mapping repositories");
                EnsureMappingsRepository();
                shouldRebuildMappings=true;
            }

            if (changeEventArgs.AddedDefinitions.Any(def => def.Exports<IOntologyProvider>()))
            {
                LogTo.Info("Refreshing ontology providers");
                EnsureOntologyProvider();
                shouldRebuildMappings=true;
            }

            if (shouldRebuildMappings)
            {
                EnsureMappingsRebuilt();
            }
        }

        private void EnsureOntologyProvider()
        {
            if (_importedOntologies.Count()==1)
            {
                _actualOntologyProvider=_importedOntologies.Single();
            }
            else
            {
                _actualOntologyProvider=new CompoundOntologyProvider(_importedOntologies);
            }
        }

        private void EnsureMappingsRepository()
        {
            _actualMappingsRepository=new CompoundMappingsRepository(_importedMappings);
        }

        private void EnsureMappingsRebuilt()
        {
            _actualMappingsRepository.RebuildMappings(new MappingContext(_actualOntologyProvider,_defaultGraphSelector));
        }

        private void EnsureComplete()
        {
            if (_entitySourceFactory==null)
            {
                throw new InvalidOperationException("Entity source factory wasn't set");
            }

            if (_baseUriSelector==null)
            {
                LogTo.Warn("No Base URI Selection Policy. It will not be possible to use relative URIs");
            }
        }
        #endregion
    }
}