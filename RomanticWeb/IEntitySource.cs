using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>
    /// A source for triples, loaded from physical triple stores
    /// </summary>
    public interface IEntitySource
    {
        [Obsolete]
        IEnumerable<Tuple<Node,Node,Node>> GetNodesForQuery(string sparqlConstruct);

        /// <summary>
        /// Loads an entity into the given <see cref="IEntityStore"/>
        /// </summary>
        void LoadEntity(IEntityStore store,EntityId entityId);

        /// <summary>
        /// Checks if an Entity with a given Id exists
        /// </summary>
        bool EntityExist(EntityId entityId);
    }
}