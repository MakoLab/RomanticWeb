using System;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>
    /// Allows changing the algorithm used to select a named graph
    /// </summary>
    public interface ISourceGraphSelectionOverride
    {
        /// <summary>
        /// Gets the select graph function.
        /// </summary>
        Func<INamedGraphSelector,Uri> SelectGraph { get; }
    }
}