using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public class EntityContextFactory : IEntityContextFactory
    {
        private readonly CompositionContainer _container;

        private Func<IEntityStore> _entityStoreFactory=()=>new EntityStore(); 

        public EntityContextFactory()
        {
            var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
            _container = new CompositionContainer(catalog, true);
        }

        public IEntityContext CreateContext()
        {
            var entitySourceFactory=_container.GetExportedValue<Func<IEntitySource>>();
            var mappings = new CompoundMappingsRepository(_container.GetExportedValues<IMappingsRepository>());
            var ontologies = new CompoundOntologyProvider(_container.GetExportedValues<IOntologyProvider>());

            return new EntityContext(this,mappings,ontologies,_entityStoreFactory(),entitySourceFactory());
        }

        public IEntityContextFactory WithEntitySource(Func<IEntitySource> entitySource)
        {
            _container.ComposeExportedValue(entitySource);
            return this;
        }

        public void SatisfyImports(object obj)
        {
            _container.ComposeParts(obj);
        }

        public IEntityContextFactory WithOntology(IOntologyProvider ontologyProvider)
        {
            _container.ComposeExportedValue(ontologyProvider);
            return this;
        }

        public IEntityContextFactory WithMappings(IMappingsRepository mappingsRepository)
        {
            _container.ComposeExportedValue(mappingsRepository);
            return this;
        }

        public IEntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory)
        {
            _entityStoreFactory=entityStoreFactory;
            return this;
        }
    }
}