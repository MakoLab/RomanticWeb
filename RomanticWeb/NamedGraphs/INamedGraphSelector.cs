using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    public interface INamedGraphSelector
    {
        Uri SelectGraph(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping predicate);
    }
}