using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    internal class NamedGraphSelector:INamedGraphSelector
    {
        public Uri SelectGraph(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping predicate)
        {
            return entityId.Uri;
        }
    }
}