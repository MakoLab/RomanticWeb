using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Configuration;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public class EntityContextFactory : IEntityContextFactory
    {
        #region Fields
        private static readonly object OntologiesLocker = new Object();
        private static IEnumerable<IOntologyProvider> importedOntologies;

        private readonly RdfTypeCache _matcher = new RdfTypeCache();
        private readonly IList<IConvention> _conventions;
        private readonly MappingsRepository _mappingsRepository;
        private bool _isInitialized;
        private Func<IEntitySource> _entitySourceFactory;
        private MappingContext _mappingContext;
        private Func<IEntityStore> _entityStoreFactory = () => new EntityStore();
        private CompoundOntologyProvider _actualOntologyProvider;
        private IBaseUriSelectionPolicy _baseUriSelector;
        private INamedGraphSelector _namedGraphSelector;
        private Uri _metaGraphUri;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
        {
            // todo: change how defaults are set
            _namedGraphSelector = new NamedGraphSelector();
            _mappingsRepository = new MappingsRepository();
            _mappingsRepository.AddVisitor(_matcher);

            WithMappings(DefaultMappings);
            _conventions = CreateDefaultConventions().ToList();
            LogTo.Info("Created entity context factory");
        }

        #endregion

        #region Properties
        /// <inheritdoc/>
        public IOntologyProvider Ontologies
        {
            get
            {
                EnsureInitialized();
                return _actualOntologyProvider;
            }
        }

        /// <inheritdoc/>
        public IMappingsRepository Mappings
        {
            get
            {
                EnsureInitialized();
                return _mappingsRepository;
            }
        }

        /// <inheritdoc/>
        public IList<IConvention> Conventions
        {
            get
            {
                return _conventions;
            }
        }

        private IEnumerable<IOntologyProvider> ImportedOntologies
        {
            get
            {
                if (importedOntologies == null)
                {
                    lock (OntologiesLocker)
                    {
                        importedOntologies = ContainerFactory.GetInstancesImplementing<IOntologyProvider>();
                    }
                }

                return importedOntologies;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a factory defined in the configuration section.
        /// </summary>
        public static EntityContextFactory FromConfiguration(string factoryName)
        {
            var configuration=ConfigurationSectionHandler.Default.Factories[factoryName];
            var ontologies=from element in configuration.Ontologies.Cast<OntologyElement>()
                           select new Ontology(element.Prefix,element.Uri);
            var mappingAssemblies=from element in configuration.MappingAssemblies.Cast<MappingAssemblyElement>()
                                  select Assembly.Load(element.Assembly);

            var entityContextFactory=new EntityContextFactory().WithOntology(new OntologyProviderBase(ontologies)).WithMappings(m =>
                {
                    foreach (var mappingAssembly in mappingAssemblies)
                    {
                        m.Fluent.FromAssembly(mappingAssembly);
                        m.Attributes.FromAssembly(mappingAssembly);
                    }
                }).WithMetaGraphUri(configuration.MetaGraphUri);
            if (configuration.BaseUris.Default!=null)
            {
                entityContextFactory.WithBaseUri(b => b.Default.Is(configuration.BaseUris.Default));
            }

            return entityContextFactory;
        }

        /// <summary>Creates a new instance of entity context.</summary>
        public IEntityContext CreateContext()
        {
            LogTo.Debug("Creating entity context");

            EnsureComplete();
            EnsureInitialized();
            _mappingContext=new MappingContext(_actualOntologyProvider);

            var entitySource=_entitySourceFactory();
            entitySource.MetaGraphUri=_metaGraphUri;

            return new EntityContext(
                this,
                Mappings,
                _mappingContext,
                _entityStoreFactory(),
                entitySource,
                _baseUriSelector,
                _namedGraphSelector,
                _matcher);
        }

        /// <summary>Includes a given <see cref="IEntitySource" /> in context that will be created.</summary>
        /// <param name="entitySource">Target entity source.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _entitySourceFactory = entitySource;
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
            _entityStoreFactory = entityStoreFactory;
            return this;
        }

        /// <summary>Exposes the method to register mapping repositories.</summary>
        /// <param name="buildMappings">Delegate method to be used for building mappings.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithMappings(Action<MappingBuilder> buildMappings)
        {
            var mappingBuilder = new MappingBuilder(_mappingsRepository);
            buildMappings.Invoke(mappingBuilder);

            return this;
        }

        /// <summary>Exposes a method to define how base <see cref="Uri"/>s are selected for relavitve <see cref="EntityId"/>s.</summary>
        public EntityContextFactory WithBaseUri(Action<BaseUriSelectorBuilder> setupPolicy)
        {
            var builder = new BaseUriSelectorBuilder();
            setupPolicy(builder);
            _baseUriSelector = builder.Build();
            return this;
        }

        /// <summary>Exposes a method to define how the default graph name should be obtained.</summary>
        public EntityContextFactory WithNamedGraphSelector(INamedGraphSelector namedGraphSelector)
        {
            _namedGraphSelector = namedGraphSelector;
            return this;
        }

        /// <summary>
        /// Sets the meta graph Uri
        /// </summary>
        public EntityContextFactory WithMetaGraphUri(Uri metaGraphUri)
        {
            _metaGraphUri=metaGraphUri;
            return this;
        }

        [Obsolete("To be refactorized")]
        public EntityContextFactory WithMappingModelVisitor(IMappingModelVisitor mappingModelVisitor)
        {
            _mappingsRepository.AddVisitor(mappingModelVisitor);
            return this;
        }
        #endregion

        #region Non-public methods

        internal static IEnumerable<IConvention> CreateDefaultConventions()
        {
            yield return new CollectionStorageConvention();

            // todo: introduce a kind of builder so that changing configuration is easy
            yield return new DefaultConvertersConvention()
                .SetDefault<IntegerConverter>(IntegerConverter.SupportedTypes)
                .SetDefault<DecimalConverter>(typeof(decimal))
                .SetDefault<BooleanConverter>(typeof(bool))
                .SetDefault<Base64BinaryConverter>(typeof(byte[]))
                .SetDefault<DateTimeConverter>(typeof(DateTime))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<DoubleConverter>(typeof(float), typeof(double))
                .SetDefault<GuidConverter>(typeof(Guid))
                .SetDefault<DefaultUriConverter>(typeof(Uri))
                .SetDefault<StringConverter>(typeof(string))
                .SetDefault<FallbackNodeConverter>(typeof(object));
            yield return new DefaultDictionaryKeyPredicateConvention();
            yield return new DefaultDictionaryValuePredicateConvention();
            yield return new RdfListConvention();
            yield return new EntityPropertiesConvention();
            yield return new EntityIdPropertiesConvention();
        }

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
            EnsureMappingsRebuilt();
            EnsureNamedGraphSelector();
            _isInitialized = true;
        }

        private void EnsureOntologyProvider()
        {
            if (_actualOntologyProvider == null)
            {
                _actualOntologyProvider = new CompoundOntologyProvider(ImportedOntologies);
            }
        }

        private void EnsureMappingsRebuilt()
        {
            var mappingContext = new MappingContext(_actualOntologyProvider, _conventions);
            _mappingsRepository.RebuildMappings(mappingContext);
        }

        private void EnsureNamedGraphSelector()
        {
            if (_namedGraphSelector == null)
            {
                _namedGraphSelector = new NamedGraphSelector();
            }
        }

        private void EnsureComplete()
        {
            if (_entitySourceFactory == null)
            {
                throw new InvalidOperationException("Entity source factory wasn't set.");
            }

            if (_baseUriSelector == null)
            {
                LogTo.Warn("No Base URI Selection Policy. It will not be possible to use relative URIs");
            }
        }
        #endregion
    }
}