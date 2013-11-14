namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A map, which allows changing the named graph, which will be used for mapped triples
    /// </summary>
    public interface INamedGraphSelectingMap
    {
        /// <summary>
        /// Gets or sets the graph selector
        /// </summary>
        IGraphSelectionStrategy GraphSelector { get; set; }
    }
}