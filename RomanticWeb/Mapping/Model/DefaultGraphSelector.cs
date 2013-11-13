using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Simply uses the entity's id for graph Uri
    /// </summary>
    public class DefaultGraphSelector:IGraphSelectionStrategy
    {
        public Uri SelectGraph(EntityId entityId)
        {
            return entityId.Uri;
        }
    }
}