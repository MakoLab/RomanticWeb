using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    public interface IEntityProxy
    {
        NamedGraphSelectionParameters NamedGraphSelectionParameters { get; }

        EntityId Id { get; }

        IEntityMapping EntityMapping { get; }

        void OverrideNamedGraphSelection(NamedGraphSelectionParameters parametersOverride);
    }
}