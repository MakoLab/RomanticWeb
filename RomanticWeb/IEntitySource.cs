using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Linq;
using RomanticWeb.Linq.Model;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>A source for triples, loaded from physical triple stores.</summary>
    public interface IEntitySource
    {
        /// <summary>Loads an entity into the given <see cref="IEntityStore"/></summary>
        void LoadEntity(IEntityStore store,EntityId entityId);

        /// <summary>Checks if an Entity with a given Id exists</summary>
        bool EntityExist(EntityId entityId);

        /// <summary>Executes given query model.</summary>
        /// <param name="queryModel">Query model to be executed.</param>
        /// <returns>Enumeration of entity quads beeing a result of the query.</returns>
        IEnumerable<EntityQuad> ExecuteEntityQuery(QueryModel queryModel);

        void ApplyChanges(DatasetChanges datasetChanges);
    }
}