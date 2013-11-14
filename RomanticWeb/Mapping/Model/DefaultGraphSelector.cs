using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Simply uses the entity's id for graph Uri
    /// </summary>
    public class DefaultGraphSelector:IGraphSelectionStrategy
    {
        /// <inheritdoc />
        /// <returns>the <see cref="EntityId.Uri"/></returns>
        public Uri SelectGraph(EntityId entityId)
        {
            return entityId.Uri;
        }
    }
}