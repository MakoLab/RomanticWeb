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
        private const string EntityStoreServiceName = "EntityStore";

        private readonly IList<Action<IServiceRegistry>> _registrations = new List<Action<IServiceRegistry>>(16);
        private readonly IDictionary<Type, object> _instances = new Dictionary<Type, object>();
        
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
        protected void EntityContext<TContext>() where TContext : IEntityContext
        {
            AddRegistration<IEntityContext, TContext>();
        }

        /// <summary>
        /// Sets the <see cref="IBlankNodeIdGenerator"/> implementation type
        /// </summary>
        protected void BlankNodeIdGenerator<TGenerator>() where TGenerator : IBlankNodeIdGenerator
        {
            AddRegistration<IBlankNodeIdGenerator, TGenerator>();
        }

        /// <summary>
        /// Adds a <see cref="IOntologyProvider"/> 
        /// </summary>
        protected void Ontology<TProvider>() where TProvider : IOntologyProvider
        {
            AddNamedRegistration<IOntologyProvider, TProvider>();
        }

        /// <summary>
        /// Sets the <see cref="INamedGraphSelector"/> implementation type
        /// </summary>
        protected void NamedGraphSelector<TSelector>() where TSelector : INamedGraphSelector
        {
            AddRegistration<INamedGraphSelector, TSelector>();
        }

        /// <summary>
        /// Sets the <see cref="IEntityStore"/> implementation type
        /// </summary>
        protected void EntityStore<TStore>() where TStore : IEntityStore
        {
            AddRegistration<IEntityStore, TStore>(EntityStoreServiceName);
        }

        /// <summary>
        /// Adds <see cref="IConvention"/> implementation type, with optional setup
        /// </summary>
        protected void Convention<TConvention>(Action<TConvention> setup = null) where TConvention : IConvention, new()
        {
            var convention = AddInstanceRegistration<IConvention, TConvention>();

            if (setup != null)
            {
                setup(convention);
            }
        }

        /// <summary>
        /// Sets the <see cref="IFallbackNodeConverter"/> implementation type
        /// </summary>
        protected void FallbackNodeConverter<TConverter>() where TConverter : IFallbackNodeConverter
        {
            AddRegistration<IFallbackNodeConverter, TConverter>(lifetime: new PerContainerLifetime());
        }

        /// <summary>
        /// Adds a <see cref="IMappingModelVisitor"/>
        /// </summary>
        protected void MappingModelVisitor<TVisitor>() where TVisitor : IMappingModelVisitor
        {
            AddNamedRegistration<IMappingModelVisitor, TVisitor>(new PerContainerLifetime());
        }

        /// <summary>
        /// Sets the <see cref="IRdfTypeCache"/> implementation type
        /// </summary>
        protected void RdfTypeCache<TCache>() 
            where TCache : IRdfTypeCache
        {
            AddRegistration<IRdfTypeCache, TCache>(lifetime: new PerContainerLifetime());
        }

        /// <summary>
        /// Adds a <see cref="IMappingProviderVisitor"/>
        /// </summary>
        protected void MappingProviderVisitor<TVisitor>() where TVisitor : IMappingProviderVisitor
        {
            var chain = AddInstanceRegistration<MappingProviderVisitorChain, MappingProviderVisitorChain>();

            chain.Add<TVisitor>();
            _registrations.Add(registry => registry.Register<TVisitor>());
        }

        private void AddNamedRegistration<TService, TImpl>(ILifetime lifetime = null) 
            where TImpl : TService
        {
            AddRegistration<TService, TImpl>(typeof(TImpl).FullName, lifetime);   
        }

        private void AddRegistration<TService, TImpl>(string name = null, ILifetime lifetime = null)
            where TImpl : TService
        {
            _registrations.Add(registry => registry.Register<TService, TImpl>(name ?? string.Empty, lifetime));
        }

        private TImpl AddInstanceRegistration<TService, TImpl>() where TImpl : TService, new()
        {
            if (!_instances.ContainsKey(typeof(TImpl)))
            {
                var convention = new TImpl();
                _registrations.Add(registry => registry.RegisterInstance<TService>(convention, typeof(TImpl).FullName));
                _instances[typeof(TImpl)] = convention;
            }

            return (TImpl)_instances[typeof(TImpl)];
        }
    }
}