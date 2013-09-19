using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Fluent
{
	public class NamedGraphPart
	{
		private readonly PredicatePart _predicatePart;
		private readonly PropertyMap _propertyMap;

		public NamedGraphPart(PredicatePart predicatePart, PropertyMap propertyMap)
		{
			this._predicatePart = predicatePart;
			this._propertyMap = propertyMap;
		}

		public PredicatePart SelectedBy<T>() where T : IGraphSelectionStrategy
		{
			throw new NotImplementedException();
		}

		public PredicatePart SelectedBy(Func<EntityId, Uri> createGraphUri)
		{
			this._propertyMap.GraphSelector = new FuncGraphSelector(createGraphUri);
			return this._predicatePart;
		}
	}
}