using System;

namespace RomanticWeb.Mapping.Model
{
    public interface IPropertyMapping
	{
		Uri Uri { get; }

		IGraphSelectionStrategy GraphSelector { get; }

		bool UsesUnionGraph { get; }

		string Name { get; }

        bool IsCollection { get; }
	}
}