using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.Configuration;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public class EntityContextFactory : IEntityContextFactory
    {
        private readonly IServiceContainer _container;

        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
            : this(new ServiceContainer())
        {
            LogTo.Info("Created entity context factory");

            WithMappings(DefaultMappings);
        }

        internal EntityContextFactory(IServiceContainer container)
        {
            _container = container;
            _container.RegisterAssembly(GetType().Assembly);
            _container.RegisterInstance<IEntityContextFactory>(this);
        }

        /// <inheritdoc/>
        public IOntologyProvider Ontologies
        {
            get
            {
                return _container.GetInstance<IOntologyProvider>();
            }
        }

        /// <inheritdoc/>
        public IMappingsRepository Mappings
        {
            get
            {
                return _container.GetInstance<IMappingsRepository>();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IConvention> Conventions
        {
            get
            {
                return _container.GetAllInstances<IConvention>();
            }
        }

        public IFallbackNodeConverter FallbackNodeConverter
        {
            get
            {
                return _container.GetInstance<IFallbackNodeConverter>();
            }
        }

        /// <summary>
        /// Creates a factory defined in the configuration section.
        /// </summary>
        public static EntityContextFactory FromConfiguration(string factoryName)
        {
            var configuration = ConfigurationSectionHandler.Default.Factories[factoryName];
            var ontologies = from element in configuration.Ontologies.Cast<OntologyElement>()
                             select new Ontology(element.Prefix, element.Uri);
            var mappingAssemblies = from element in configuration.MappingAssemblies.Cast<MappingAssemblyElement>()
                                    select Assembly.Load(element.Assembly);

            var entityContextFactory = new EntityContextFactory().WithOntology(new OntologyProviderBase(ontologies)).WithMappings(m =>
                {
                    foreach (var mappingAssembly in mappingAssemblies)
                    {
                        m.Fluent.FromAssembly(mappingAssembly);
                        m.Attributes.FromAssembly(mappingAssembly);
                    }
                }).WithMetaGraphUri(configuration.MetaGraphUri);
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
            
            return _container.GetInstance<IEntityContext>();
        }

        /// <summary>Includes a given <see cref="IEntitySource" /> in context that will be created.</summary>
        /// <param name="entitySource">Target entity source.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _container.Register(f => entitySource(), "EntitySource");
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

        /// <summary>Includes a given <see cref="IEntityStore" /> in context that will be created.</summary>
        /// <param name="entityStoreFactory">Target entity store.</param>
        /// <returns>This <see cref="EntityContextFactory" /> </returns>
        public EntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory)
        {
            _container.Register(f => entityStoreFactory());
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

        internal EntityContextFactory WithDependencies<T>() where T : ICompositionRoot, new()
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