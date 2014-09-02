using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>Represents an in-memory quad cache, which are organized in per-entity quads.</summary>
    public interface IEntityStore
    {
        /// <summary>Gets all quads from the store.</summary>
        IEnumerable<EntityQuad> Quads { get; }

        /// <summary>Gets all objects for predicate for a given entity.</summary>
        IEnumerable<Node> GetObjectsForPredicate(EntityId entityId, Uri predicate, Uri graph);

        /// <summary>Gets all quads describing given entity.</summary>
        /// <param name="entityId">Entity identifier for which to retrieve quads.</param>
        /// <param name="includeBlankNodes">Includes all quads for blank-node entities.</param>
        /// <returns>Enumeration of quads describing given entity.</returns>
        IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId, bool includeBlankNodes = true);

        void ReplacePredicateValues(EntityId id, Node predicate, Node[] newValues, Uri graph);

        void AssertEntity(EntityId entityId, IEnumerable<EntityQuad> quads);

        void Delete(EntityId entityId, DeleteBehaviour deleteBehaviour);

        /// <summary>Forces the store to use current state as it's initial state.</summary>
        void ResetState();
    }
}