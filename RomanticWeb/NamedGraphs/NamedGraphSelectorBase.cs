using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    public abstract class NamedGraphSelectorBase : INamedGraphSelector
    {
        public abstract Uri SelectGraph(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping predicate);
    }
}