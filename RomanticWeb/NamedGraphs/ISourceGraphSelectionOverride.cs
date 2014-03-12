using System;

namespace RomanticWeb.NamedGraphs
{
    public interface ISourceGraphSelectionOverride
    {
        Func<INamedGraphSelector,Uri> SelectGraph { get; }
    }
}