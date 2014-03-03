namespace RomanticWeb.Entities
{
    public interface IEntityProxy
    {
        NamedGraphSelectionParameters NamedGraphSelectionParameters { get; }

        void OverrideNamedGraphSelection(NamedGraphSelectionParameters parametersOverride);
    }
}