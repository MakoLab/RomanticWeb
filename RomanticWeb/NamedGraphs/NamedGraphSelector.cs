using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    internal class NamedGraphSelector : INamedGraphSelector
    {
        public Uri SelectGraph(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping predicate)
        {
            if (entityId is BlankId)
            {
                EntityId nonBlankId = ((BlankId)entityId).RootEntityId;
                if (nonBlankId != null)
                {
                    entityId = nonBlankId;
                }
            }

            return entityId.Uri;
        }
    }
}