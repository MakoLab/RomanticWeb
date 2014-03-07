using System;

namespace RomanticWeb.NamedGraphs
{
    public interface SourceGraphSelectionOverride
    {
        Func<INamedGraphSelector,Uri> SelectGraph { get; }
    }
}