using System;

namespace RomanticWeb
{
    public interface IPropertyMapping
    {
        Uri Uri { get; }
        IGraphSelectionStrategy GraphSelector { get; }
        bool UsesUnionGraph { get; }
        string Name { get; }
    }
}