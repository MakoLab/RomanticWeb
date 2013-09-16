using System;
using NullGuard;

namespace RomanticWeb.Mapping
{
	public class PropertyMapping : IPropertyMapping
	{
		public PropertyMapping(string name, Uri uri, [AllowNull] IGraphSelectionStrategy graphSelector)
		{
			Name = name;
			Uri = uri;
			GraphSelector = graphSelector;
		}

		public Uri Uri { get; private set; }

		public IGraphSelectionStrategy GraphSelector { [return:AllowNull] get; private set; }

		public bool UsesUnionGraph { get; private set; }

		public string Name { get; private set; }
	}
}