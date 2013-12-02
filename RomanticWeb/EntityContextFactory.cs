using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public class EntityContextFactory : IEntityContextFactory
    {
        private readonly CompositionContainer _container;
        private bool _isInitialized;

        [ImportMany(typeof(IOntologyProvider), AllowRecomposition = true)]
        private IEnumerable<IOntologyProvider> _importedOntologies;

        [ImportMany(typeof(IMappingsRepository), AllowRecomposition = true)]
        private IEnumerable<IMappingsRepository> _importedMappings;

        private Func<IEntitySource> _entitySourceFactory;
        private MappingContext _mappingContext;
        private Func<IEntityStore> _entityStoreFactory = () => new EntityStore();
        private IGraphSelectionStrategy _defaultGraphSelector = new DefaultGraphSelector();
        private IMappingsRepository _actualMappingsRepository;
        private IOntologyProvider _actualOntologyProvider;

        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
        {
            var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.GetPrimaryAssemblyPath());
            _container = new CompositionContainer(catalog, true);
            catalog.Changed += CatalogChanged;
        }

        /// <inheritdoc />
        public IOntologyProvider Ontologies
        {
            get
            {
                EnsureInitialized();
                return _actualOntologyProvider;
            }
        }

        /// <inheritdoc />
        public IMappingsRepository Mappings
        {
            get
            {
                EnsureInitialized();
                return _actualMappingsRepository;
            }
        }

        /// <summary>
        /// Creates a new instance of entity context
        /// </summary>
        public IEntityContext CreateContext()
        {
            EnsureInitialized();
            _mappingContext = new MappingContext(_actualOntologyProvider, _defaultGraphSelector);
            _actualMappingsRepository.RebuildMappings(_mappingContext);

            return new EntityContext(this, Mappings, _mappingContext, _entityStoreFactory(), _entitySourceFactory());
        }

        public EntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _entitySourceFactory=entitySource;
            return this;
        }

        /// <inheritdoc />
        public void SatisfyImports(object obj)
        {
            _container.ComposeParts(obj);
        }

        public EntityContextFactory WithOntology(IOntologyProvider ontologyProvider)
        {
            _container.ComposeExportedValue(ontologyProvider);
            return this;
        }

        /// <summary>
        /// Exposes the method to register mapping repositories
        /// </summary>
        public EntityContextFactory WithMappings(Action<MappingBuilder> buildMappings)
        {
            var mappingBuilder = new MappingBuilder();
            buildMappings(mappingBuilder);

            foreach (var mappingsRepository in mappingBuilder)
            {
                _container.ComposeExportedValue(mappingsRepository);
            }

            return this;
        }

        public EntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory)
        {
            _entityStoreFactory = entityStoreFactory;
            return this;
        }

        public EntityContextFactory WithDefaultGraphSelector(IGraphSelectionStrategy graphSelector)
        {
            _defaultGraphSelector = graphSelector;
            return this;
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            var batch = new CompositionBatch();
            batch.AddPart(this);

            _container.Compose(batch);
            _actualOntologyProvider = new CompoundOntologyProvider(_importedOntologies);
            _actualMappingsRepository = new CompoundMappingsRepository(_importedMappings);

            _isInitialized=true;
        }

        private void CatalogChanged(object sender, ComposablePartCatalogChangeEventArgs changeEventArgs)
        {
            if (changeEventArgs.AddedDefinitions.Any(def => def.Exports<IMappingsRepository>()))
            {
                _actualMappingsRepository = new CompoundMappingsRepository(_importedMappings);
            }

            if (changeEventArgs.AddedDefinitions.Any(def => def.Exports<IOntologyProvider>()))
            {
                _actualOntologyProvider = new CompoundOntologyProvider(_importedOntologies);
            }
        }
    }
}