using System;

namespace RomanticWeb.Mapping
{
	public class NamedGraphPart
	{
		private readonly PredicatePart _predicatePart;
		private readonly PropertyMap _propertyMap;

		public NamedGraphPart(PredicatePart predicatePart, PropertyMap propertyMap)
		{
			_predicatePart = predicatePart;
			_propertyMap = propertyMap;
		}

		public PredicatePart SelectedBy<T>() where T : IGraphSelectionStrategy
		{
			throw new NotImplementedException();
		}

		public PredicatePart SelectedBy(Func<EntityId, Uri> createGraphUri)
		{
			_propertyMap.GraphSelector = new FuncGraphSelector(createGraphUri);
			return _predicatePart;
		}
	}
}