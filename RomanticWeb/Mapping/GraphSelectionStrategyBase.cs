using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Defines method for selecting named graph URI based on <see cref="EntityId"/> 
    /// </summary>
    public abstract class GraphSelectionStrategyBase
    {
        internal Uri SelectGraph(EntityId entityId)
        {
            if (entityId is BlankId)
            {
                var parentEntityId=((BlankId)entityId).RootEntityId;

                if (parentEntityId==null)
                {
                    throw new ArgumentException("Blank node must have parent id","entityId");
                }

                return SelectGraph(parentEntityId);
            }

            return GetGraphForEntityId(entityId);
        }

        /// <summary>
        /// Gets a named graph URI for a given entity
        /// </summary>
        /// <param name="entityId">Entity's identifier</param>
        protected abstract Uri GetGraphForEntityId(EntityId entityId);
    }
}