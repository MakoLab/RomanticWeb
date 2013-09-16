namespace RomanticWeb.Mapping.Model
{
    using System;

    using NullGuard;

    public class PropertyMapping : IPropertyMapping
	{
		public PropertyMapping(string name, Uri uri, [AllowNull] IGraphSelectionStrategy graphSelector)
		{
			this.Name = name;
			this.Uri = uri;
			this.GraphSelector = graphSelector;
		}

		public Uri Uri { get; private set; }

		public IGraphSelectionStrategy GraphSelector { [return:AllowNull] get; private set; }

		public bool UsesUnionGraph { get; private set; }

		public string Name { get; private set; }
	}
}