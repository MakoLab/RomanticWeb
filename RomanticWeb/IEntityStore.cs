using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>
    /// Represents an in-memory quad cache, which are organized in per-entity quads 
    /// </summary>
    public interface IEntityStore
    {
        /// <summary>
        /// Gets all quads from the store
        /// </summary>
        IEnumerable<EntityTriple> Quads { get; }

        /// <summary>
        /// Gets all objects for predicate for a given entity
        /// </summary>
        IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate);

        /// <summary>
        /// Adds a triple to the store
        /// </summary>
        void AssertEntity(EntityId entityId, IEnumerable<EntityTriple> entityTriples);

        void ReplacePredicateValue(EntityId id,Node propertyUri,Node valueNode,Uri graphUri);
    }
}