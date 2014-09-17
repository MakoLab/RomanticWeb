using System;
using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb.ComponentModel
{
    /// <summary>
    /// Base class for changing Romantic Web components
    /// </summary>
    public abstract class CompositionRootBase : ICompositionRoot
    {
        private readonly IList<Action<IServiceRegistry>> _registrations = new List<Action<IServiceRegistry>>(16);

        private MappingProviderVisitorChain _mappingProviderVisitorChain;

        private MappingProviderVisitorChain MappingProviderVisitorChain
        {
            get
            {
                if (_mappingProviderVisitorChain == null)
                {
                    _mappingProviderVisitorChain = new MappingProviderVisitorChain();
                    AddInstanceRegistration(_mappingProviderVisitorChain, "Mapping provider visitors " + Guid.NewGuid());
                }

                return _mappingProviderVisitorChain;
            }
        }

        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            foreach (var registration in _registrations)
            {
                registration(serviceRegistry);
            }
        }

        /// <summary>
        /// Sets the <see cref="IEntityContext"/> implementation type
        /// </summary>
        protected void EntityContext<TContext>() 
            where TContext : IEntityContext
        {
            TransientComponent<IEntityContext, TContext>();
        }

        /// <summary>
        /// Sets the <see cref="IBlankNodeIdGenerator"/> implementation type
        /// </summary>
        protected void BlankNodeIdGenerator<TGenerator>() 
            where TGenerator : IBlankNodeIdGenerator
        {
            SharedComponent<IBlankNodeIdGenerator, TGenerator>();
        }

        /// <summary>
        /// Adds a <see cref="IOntologyProvider"/> 
        /// </summary>
        protected void Ontology<TProvider>() 
            where TProvider : IOntologyProvider
        {
            SharedComponent<IOntologyProvider, TProvider>(typeof(TProvider).FullName);
        }

        /// <summary>
        /// Sets the <see cref="INamedGraphSelector"/> implementation type
        /// </summary>
        protected void NamedGraphSelector<TSelector>() 
            where TSelector : INamedGraphSelector
        {
            SharedComponent<INamedGraphSelector, TSelector>();
        }

        /// <summary>
        /// Sets the <see cref="IEntityStore"/> implementation type
        /// </summary>
        protected void EntityStore<TStore>()
            where TStore : IEntityStore
        {
            PerEntityContextComponent<IEntityStore, TStore>();
        }

        /// <summary>
        /// Adds <see cref="IConvention"/> implementation type, with optional setup
        /// </summary>
        protected void Convention<TConvention>(Action<TConvention> setup = null) 
            where TConvention : IConvention, new()
        {
            _registrations.Add(registry => registry.Register<TConvention>(new PerContainerLifetime()));
            _registrations.Add(registry => registry.Register<IConvention>(factory => CreateConvention<TConvention>(factory), typeof(TConvention).FullName, new PerContainerLifetime()));

            if (setup != null)
            {
                SharedComponent(setup, typeof(TConvention).FullName + " setup " + Guid.NewGuid());
            }
        }

        /// <summary>
        /// Sets the <see cref="IFallbackNodeConverter"/> implementation type
        /// </summary>
        protected void FallbackNodeConverter<TConverter>() 
            where TConverter : IFallbackNodeConverter
        {
            SharedComponent<IFallbackNodeConverter, TConverter>();
        }

        /// <summary>
        /// Adds a <see cref="IMappingModelVisitor"/>
        /// </summary>
        protected void MappingModelVisitor<TVisitor>() 
            where TVisitor : IMappingModelVisitor
        {
            SharedComponent<IMappingModelVisitor, TVisitor>();
        }

        /// <summary>
        /// Adds a <see cref="IOntologyLoader"/>
        /// </summary>
        protected void OntologyLoader<TLoader>() 
            where TLoader : IOntologyLoader
        {
            SharedComponent<IOntologyLoader, TLoader>();
        }

        /// <summary>
        /// Sets the <see cref="IRdfTypeCache"/> implementation type
        /// </summary>
        protected void RdfTypeCache<TCache>() 
            where TCache : IRdfTypeCache
        {
            SharedComponent<IRdfTypeCache, TCache>();
        }

        /// <summary>
        /// Adds a <see cref="IMappingProviderVisitor"/>
        /// </summary>
        protected void MappingProviderVisitor<TVisitor>() 
            where TVisitor : IMappingProviderVisitor
        {
            MappingProviderVisitorChain.Add<TVisitor>();
            _registrations.Add(registry => registry.Register<TVisitor>(new PerContainerLifetime()));
        }

        /// <summary>
        /// Registers a custom dependency type. It will be shared between all <see cref="EntityContext"/>s
        /// </summary>
        /// <typeparam name="TComponent">The type of the dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">Optional name.</param>
        protected void SharedComponent<TComponent, TImplementation>(string name = null) 
            where TImplementation : TComponent
        {
            AddRegistration<TComponent, TImplementation>(name, new PerContainerLifetime());
        }

        /// <summary>
        /// Registers a custom dependency instance. It will be shared between all <see cref="EntityContext"/>s
        /// </summary>
        /// <typeparam name="TComponent">The type of the dependency.</typeparam>
        /// <param name="instance">An instance of <typeparamref name="TComponent"/></param>
        /// <param name="name">Optional name.</param>
        protected void SharedComponent<TComponent>(TComponent instance, string name = null)
        {
            AddInstanceRegistration(instance, name);
        }

        /// <summary>
        /// Registers a custom dependency type. A new instance will be created for each <see cref="EntityContext"/>
        /// </summary>
        /// <typeparam name="TComponent">The type of the dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">Optional name.</param>
        protected void TransientComponent<TComponent, TImplementation>(string name = null) 
            where TImplementation : TComponent
        {
            AddRegistration<TComponent, TImplementation>(name, new PerRequestLifeTime());
        }

        protected void PerEntityContextComponent<TComponent, TImplementation>(string name = null)
            where TImplementation : TComponent
        {
            AddRegistration<TComponent, TImplementation>(name, new PerScopeLifetime());
        }

        private void AddRegistration<TComponent, TImplementation>(string name, ILifetime lifetime)
            where TImplementation : TComponent
        {
            _registrations.Add(registry => registry.Register<TComponent, TImplementation>(name ?? string.Empty, lifetime));
        }

        private void AddInstanceRegistration<TComponent>(TComponent instance, string name)
        {
            _registrations.Add(registry => registry.RegisterInstance(instance, name ?? string.Empty));
        }

        private TConvention CreateConvention<TConvention>(IServiceFactory factory)
            where TConvention : IConvention
        {
            var convention = factory.GetInstance<TConvention>();
            foreach (var setup in factory.GetAllInstances<Action<TConvention>>())
            {
                setup(convention);
            }

            return convention;
        }
    }
}