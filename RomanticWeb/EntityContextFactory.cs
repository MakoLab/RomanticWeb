using System;
using System.Collections.Generic;
using Anotar.NLog;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public class EntityContextFactory:IEntityContextFactory
    {
        #region Fields
        private static readonly object MappingsLocker=new Object();
        private static readonly object OntologiesLocker=new Object();
        private static IEnumerable<IOntologyProvider> importedOntologies;
        private static IEnumerable<IMappingsRepository> importedMappings;

        private readonly ConverterCatalog _conveters=new ConverterCatalog();
        private readonly MappingBuilder _mappingBuilder=new MappingBuilder();
        private bool _isInitialized;
        private Func<IEntitySource> _entitySourceFactory;
        private MappingContext _mappingContext;
        private Func<IEntityStore> _entityStoreFactory=() => new EntityStore();
        private CompoundMappingsRepository _actualMappingsRepository;
        private CompoundOntologyProvider _actualOntologyProvider;
        private IBaseUriSelectionPolicy _baseUriSelector;
        private INamedGraphSelector _namedGraphSelector;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
        {
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

        public IConverterCatalog Converters
        {
            get
            {
                return _conveters;
            }
        }

        private IEnumerable<IOntologyProvider> ImportedOntologies
        {
            get
            {
                if (importedOntologies==null)
                {
                    lock (OntologiesLocker)
                    {
                        importedOntologies=ContainerFactory.GetInstancesImplementing<IOntologyProvider>();
                    }
                }

                return importedOntologies;
            }
        }

        private IEnumerable<IMappingsRepository> ImportedMappings
        {
            get
            {
                if (importedMappings==null)
                {
                    lock (MappingsLocker)
                    {
                        importedMappings=ContainerFactory.GetInstancesImplementing<IMappingsRepository>();
                    }
                }

                return importedMappings;
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
            _mappingContext=new MappingContext(_actualOntologyProvider);

            return new EntityContext(
                this,
                Mappings,
                _mappingContext,
                _entityStoreFactory(),
                _entitySourceFactory(),
                _baseUriSelector,
                _namedGraphSelector);
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

        public IEntityContextFactory WithNamedGraphSelector(INamedGraphSelector namedGraphSelector)
        {
            _namedGraphSelector=namedGraphSelector;
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
            _actualMappingsRepository.RebuildMappings(new MappingContext(_actualOntologyProvider));
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