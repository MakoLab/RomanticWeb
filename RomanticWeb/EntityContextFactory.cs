using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.ComponentModel;
using RomanticWeb.Configuration;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>.</summary>
    public class EntityContextFactory : IEntityContextFactory, IComponentRegistryFacade
    {
        private static readonly object Locker = new object();
        private readonly IServiceContainer _container;
        private readonly IList<Scope> _trackedScopes = new List<Scope>();
        private bool _disposed;

        /// <summary>Creates a new instance of <see cref="EntityContextFactory"/>.</summary>
        public EntityContextFactory() : this(new ServiceContainer())
        {
        }

        internal EntityContextFactory(IServiceContainer container)
        {
            _container = container;
            _container.RegisterAssembly("RomanticWeb.dll");
            _container.Register<IEntityContextFactory>(f => this);

            WithMappings(DefaultMappings);

            LogTo.Info("Created entity context factory");
        }

        /// <inheritdoc/>
        public IOntologyProvider Ontologies { get { return new CompoundOntologyProvider(_container.GetAllInstances<IOntologyProvider>()); } }

        /// <inheritdoc/>
        public IMappingsRepository Mappings { get { return _container.GetInstance<IMappingsRepository>(); } }

        /// <inheritdoc/>
        public IEnumerable<IConvention> Conventions { get { return _container.GetAllInstances<IConvention>(); } }

        /// <inheritdoc/>
        public IFallbackNodeConverter FallbackNodeConverter { get { return _container.GetInstance<IFallbackNodeConverter>(); } }

        /// <inheritdoc/>
        public IEnumerable<IMappingModelVisitor> MappingModelVisitors { get { return _container.GetAllInstances<IMappingModelVisitor>(); } }

        /// <inheritdoc />
        public IResultTransformerCatalog TransformerCatalog { get { return _container.GetInstance<IResultTransformerCatalog>(); } }

        /// <inheritdoc />
        public INamedGraphSelector NamedGraphSelector { get { return _container.GetInstance<INamedGraphSelector>(); } }

        internal IList<Scope> TrackedScopes { get { return _trackedScopes; } }

        internal bool ThreadSafe { get; set; }

        internal bool TrackChanges { get; set; }

        /// <summary>Creates a factory defined in the configuration section.</summary>
        public static EntityContextFactory FromConfiguration(string factoryName)
        {
            var configuration = ConfigurationSectionHandler.Default.Factories[factoryName];
            var ontologies = from element in configuration.Ontologies.Cast<OntologyElement>()
                             select new Ontology(element.Prefix, element.Uri);
            var mappingAssemblies = from element in configuration.MappingAssemblies.Cast<MappingAssemblyElement>()
                                    select Assembly.Load(element.Assembly);

            var entityContextFactory = new EntityContextFactory()
                .WithOntology(new OntologyProviderBase(ontologies))
                .WithMetaGraphUri(configuration.MetaGraphUri)
                .WithMappings(m =>
                {
                    foreach (var mappingAssembly in mappingAssemblies)
                    {
                        m.Fluent.FromAssembly(mappingAssembly);
                        m.Attributes.FromAssembly(mappingAssembly);
                    }
                });
            entityContextFactory.ThreadSafe = configuration.ThreadSafe;
            entityContextFactory.TrackChanges = configuration.TrackChanges;
            if (configuration.BaseUris.Default != null)
            {
                entityContextFactory.WithBaseUri(b => b.Default.Is(configuration.BaseUris.Default));
            }

            return entityContextFactory;
        }

        /// <summary>Creates a new instance of entity context.</summary>
        public IEntityContext CreateContext()
        {
            LogTo.Debug("Creating entity context");

            lock (Locker)
            {
                var scope = _container.BeginScope();
                TrackedScopes.Add(scope);
                var context = _container.GetInstance<IEntityContext>();
                context.TrackChanges = TrackChanges;
                if (context.Store is IThreadSafeEntityStore)
                {
                    ((IThreadSafeEntityStore)context.Store).ThreadSafe = ThreadSafe;
                }

                context.Disposed += () =>
                    {
                        scope.Dispose();
                        _trackedScopes.Remove(scope);
                    };
                _container.ScopeManagerProvider.GetScopeManager().EndScope(scope);
                return context;
            }
        }

        /// <summary>Includes a given <see cref="IEntitySource" /> in context that will be created.</summary>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntitySource<TSource>() where TSource : IEntitySource
        {
            _container.Register<IEntitySource, TSource>("EntitySource");
            return this;
        }

        /// <summary>Includes a given <see cref="IOntologyProvider" /> in context that will be created.</summary>
        /// <param name="ontologyProvider">Target ontology provider.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithOntology(IOntologyProvider ontologyProvider)
        {
            // todo: get rid of Guid by refatoring how ontolgies are added
            _container.RegisterInstance(ontologyProvider, Guid.NewGuid().ToString());

            return this;
        }

        /// <summary>Exposes the method to register mapping repositories.</summary>
        /// <param name="buildMappings">Delegate method to be used for building mappings.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithMappings(Action<MappingBuilder> buildMappings)
        {
            var mappingBuilder = new MappingBuilder();
            buildMappings.Invoke(mappingBuilder);

            foreach (var source in mappingBuilder.Sources)
            {
                _container.RegisterInstance(source, source.Description);
            }

            return this;
        }

        /// <summary>Exposes a method to define how base <see cref="Uri"/>s are selected for relavitve <see cref="EntityId"/>s.</summary>
        public EntityContextFactory WithBaseUri(Action<BaseUriSelectorBuilder> setupPolicy)
        {
            var builder = new BaseUriSelectorBuilder();
            setupPolicy(builder);
            _container.RegisterInstance(builder.Build());
            return this;
        }

        /// <summary>Exposes a method to define how the default graph name should be obtained.</summary>
        public EntityContextFactory WithNamedGraphSelector(INamedGraphSelector namedGraphSelector)
        {
            _container.RegisterInstance(namedGraphSelector);
            return this;
        }

        /// <summary>
        /// Sets the meta graph Uri
        /// </summary>
        public EntityContextFactory WithMetaGraphUri(Uri metaGraphUri)
        {
            _container.RegisterInstance(metaGraphUri, "MetaGraphUri");
            return this;
        }

        /// <summary>
        /// Registers dependencies from a given <see cref="CompositionRootBase"/> type
        /// </summary>
        public EntityContextFactory WithDependencies<T>() where T : CompositionRootBase, new()
        {
            return WithDependenciesInternal<T>();
        }

        void IComponentRegistryFacade.Register<TService, TComponent>()
        {
            _container.Register<TService, TComponent>();
        }

        void IComponentRegistryFacade.Register<TService>(TService instance)
        {
            _container.RegisterInstance(instance);
        }

        /// <summary>
        /// Dispose this entity context factory and all components
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var scope in TrackedScopes.ToList())
            {
                scope.Dispose();
            }

            _container.Dispose();

            _disposed = true;
        }

        internal EntityContextFactory WithDependenciesInternal<T>() where T : ICompositionRoot, new()
        {
            _container.RegisterFrom<T>();
            return this;
        }

        private static void DefaultMappings(MappingBuilder mappings)
        {
            mappings.Fluent.FromAssemblyOf<ITypedEntity>();
            mappings.Attributes.FromAssemblyOf<ITypedEntity>();
        }
    }
}
