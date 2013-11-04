using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>
    /// A source for triples, loaded from physical triple stores
    /// </summary>
    public interface IEntitySource
    {
        /// <summary>
        /// Loads an entity into the given <see cref="IEntityStore"/>
        /// </summary>
        void LoadEntity(IEntityStore store,EntityId entityId);

        /// <summary>
        /// Checks if an Entity with a given Id exists
        /// </summary>
        bool EntityExist(EntityId entityId);

        IEnumerable<EntityTriple> ExecuteEntityQuery(SparqlQuery sparqlQuery);
    }
}