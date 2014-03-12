using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    public interface IEntityProxy
    {
        ISourceGraphSelectionOverride GraphSelectionOverride { get; }

        EntityId Id { get; }

        IEntityMapping EntityMapping { get; }

        void OverrideGraphSelection(ISourceGraphSelectionOverride graphOverride);
    }
}