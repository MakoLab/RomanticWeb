using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    internal class NamedGraphSelector : NamedGraphSelectorBase
    {
        public override Uri SelectGraph(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping predicate)
        {
            return entityId.Uri;
        }
    }
}