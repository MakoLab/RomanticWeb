using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using RomanticWeb.ComponentModel.Composition;
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
        private static IEnumerable<IOntologyProvider> _importedOntologies=null;
        private static object _ontologiesLocker=new Object();
        private static IEnumerable<IMappingsRepository> _importedMappings=null;
        private static object _mappingsLocker=new Object();

        private readonly MappingBuilder _mappingBuilder=new MappingBuilder();
        private readonly CompositionContainer _container;
        private bool _isInitialized;
        private Func<IEntitySource> _entitySourceFactory;
        private MappingContext _mappingContext;
        private Func<IEntityStore> _entityStoreFactory=() => new EntityStore();
        private GraphSelectionStrategyBase _defaultGraphSelector=new DefaultGraphSelector();
        private CompoundMappingsRepository _actualMappingsRepository;
        private CompoundOntologyProvider _actualOntologyProvider;
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

        private IEnumerable<IOntologyProvider> ImportedOntologies
        {
            get
            {
                if (_importedOntologies==null)
                {
                    lock (_ontologiesLocker)
                    {
                        _importedOntologies=ContainerFactory.GetInstancesImplementing<IOntologyProvider>();
                    }
                }

                return _importedOntologies;
            }
        }

        private IEnumerable<IMappingsRepository> ImportedMappings
        {
            get
            {
                if (_importedMappings==null)
                {
                    lock (_mappingsLocker)
                    {
                        _importedMappings=ContainerFactory.GetInstancesImplementing<IMappingsRepository>();
                    }
                }

                return _importedMappings;
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a new instance of entity context.</summary>
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
            EnsureOntologyProvider();
            if (!_actualOntologyProvider.OntologyProviders.Contains(ontologyProvider))
            {
                _actualOntologyProvider.OntologyProviders.Add(ontologyProvider);
            }

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
            EnsureMappingsRepository();
            foreach (var mappingsRepository in repositories)
            {
                if (!_actualMappingsRepository.MappingsRepositories.Contains(mappingsRepository))
                {
                    _actualMappingsRepository.MappingsRepositories.Add(mappingsRepository);
                }
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
            if (_actualOntologyProvider==null)
            {
                _actualOntologyProvider=new CompoundOntologyProvider(ImportedOntologies);
            }
        }

        private void EnsureMappingsRepository()
        {
            if (_actualMappingsRepository==null)
            {
                _actualMappingsRepository=new CompoundMappingsRepository(ImportedMappings);
            }
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