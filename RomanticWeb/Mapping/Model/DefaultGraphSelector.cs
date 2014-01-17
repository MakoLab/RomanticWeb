using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Simply uses the entity's id for graph Uri
    /// </summary>
    public class DefaultGraphSelector:GraphSelectionStrategyBase
    {
        /// <inheritdoc />
        /// <returns>the <see cref="EntityId.Uri"/></returns>
        protected override Uri GetGraphForEntityId(EntityId entityId)
        {
            return entityId.Uri;
        }
    }
}