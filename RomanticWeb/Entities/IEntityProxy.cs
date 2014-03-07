using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    public interface IEntityProxy
    {
        SourceGraphSelectionOverride GraphSelectionOverride { get; }

        EntityId Id { get; }

        IEntityMapping EntityMapping { get; }

        void OverrideGraphSelection(SourceGraphSelectionOverride graphOverride);
    }
}