namespace RomanticWeb.Mapping.Fluent
{
    public interface INamedGraphSelectingMap
    {
        IGraphSelectionStrategy GraphSelector { get; set; }
    }
}