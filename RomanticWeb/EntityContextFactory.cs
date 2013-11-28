using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        private readonly CompositionContainer _container;

        private Func<IEntityStore> _entityStoreFactory=() => new EntityStore();

        private IGraphSelectionStrategy _defaultGraphSelector=new DefaultGraphSelector();

        /// <summary>
        /// Creates a new instance of <see cref="EntityContextFactory"/>
        /// </summary>
        public EntityContextFactory()
        {
            var catalog=new DirectoryCatalog(AppDomain.CurrentDomain.GetPrimaryAssemblyPath());
            _container=new CompositionContainer(catalog,true);
        }

        /// <summary>
        /// Creates a new instance of entity context
        /// </summary>
        public IEntityContext CreateContext()
        {
            var entitySourceFactory=_container.GetExportedValue<Func<IEntitySource>>();
            var mappings=new CompoundMappingsRepository(_container.GetExportedValues<IMappingsRepository>());
            var ontologies=new CompoundOntologyProvider(_container.GetExportedValues<IOntologyProvider>());
            var mappingContext=new MappingContext(ontologies,_defaultGraphSelector);

            mappings.RebuildMappings(mappingContext);
            return new EntityContext(this,mappings,mappingContext,_entityStoreFactory(),entitySourceFactory());
        }

        public EntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _container.ComposeExportedValue(entitySource);
            return this;
        }

        public void SatisfyImports(object obj)
        {
            _container.ComposeParts(obj);
        }

        public EntityContextFactory WithOntology(IOntologyProvider ontologyProvider)
        {
            _container.ComposeExportedValue(ontologyProvider);
            return this;
        }

        public EntityContextFactory WithMappings(IMappingsRepository mappingsRepository)
        {
            _container.ComposeExportedValue(mappingsRepository);
            return this;
        }

        public EntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory)
        {
            _entityStoreFactory=entityStoreFactory;
            return this;
        }

        public EntityContextFactory WithDefaultGraphSelector(IGraphSelectionStrategy graphSelector)
        {
            _defaultGraphSelector=graphSelector;
            return this;
        }
    }
}