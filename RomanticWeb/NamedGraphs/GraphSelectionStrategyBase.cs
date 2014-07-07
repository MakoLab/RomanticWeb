using System;
using RomanticWeb.Entities;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>
    /// Defines method for selecting named graph URI based on <see cref="EntityId"/> 
    /// </summary>
    public abstract class GraphSelectionStrategyBase
    {
        internal Uri SelectGraph(EntityId entityId, Uri predicate)
        {
            var nonBlankId = entityId;
            while (nonBlankId is BlankId)
            {
                nonBlankId = ((BlankId)entityId).RootEntityId;

                if (nonBlankId == null)
                {
                    throw new ArgumentException("Blank node must have parent id", "entityId");
                }
            }

            return GetGraphForEntityId(nonBlankId, predicate);
        }

        /// <summary>
        /// Gets a named graph URI for a given entity
        /// </summary>
        /// <param name="entityId">Entity's identifier</param>
        /// <param name="predicate"></param>
        protected abstract Uri GetGraphForEntityId(EntityId entityId, Uri predicate);
    }
}