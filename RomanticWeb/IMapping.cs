using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IMapping<TEntity>
    {
        IPropertyMapping PropertyFor(string propertyName);
    }

    public interface IPropertyMapping
    {
        Uri Uri { get; }
        IGraphSelectionStrategy GraphSelector { get; }
        bool UsesUnionGraph { get; }
    }

    public interface IGraphSelectionStrategy
    {
        Uri SelectGraph(EntityId entityId);
    }
}