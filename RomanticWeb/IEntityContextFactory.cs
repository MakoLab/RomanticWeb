using System;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IEntityContextFactory
    {
        IEntityContext Create();

        IEntityContextFactory WithEntitySource(Func<IEntitySource> entitySource);

        void SatisfyImports(object obj);

        IEntityContextFactory WithOntology(IOntologyProvider ontologyProvider);

        IEntityContextFactory WithMappings(IMappingsRepository mappingsRepository);

        IEntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory);
    }
}