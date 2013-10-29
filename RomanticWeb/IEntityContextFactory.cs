using System;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext"/>
    /// </summary>
    public interface IEntityContextFactory
    {
        /// <summary>
        /// Creates a new <see cref="IEntityContext"/>
        /// </summary>
        IEntityContext CreateContext();

        IEntityContextFactory WithEntitySource(Func<IEntitySource> entitySource);

        void SatisfyImports(object obj);

        IEntityContextFactory WithOntology(IOntologyProvider ontologyProvider);

        IEntityContextFactory WithMappings(IMappingsRepository mappingsRepository);

        IEntityContextFactory WithEntityStore(Func<IEntityStore> entityStoreFactory);
    }
}