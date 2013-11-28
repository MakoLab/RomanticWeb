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
        /// Creates a new instance of entity context
        /// </summary>
        IEntityContext CreateContext();

        void SatisfyImports(object obj);
    }
}