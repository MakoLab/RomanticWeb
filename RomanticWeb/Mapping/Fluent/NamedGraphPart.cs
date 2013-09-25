using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping a property to a named graph
    /// </summary>
	public class NamedGraphPart
	{
		private readonly PredicatePart _predicatePart;
		private readonly PropertyMap _propertyMap;

        internal NamedGraphPart(PredicatePart predicatePart, PropertyMap propertyMap)
		{
			_predicatePart = predicatePart;
			_propertyMap = propertyMap;
		}

		public PredicatePart SelectedBy<T>() where T : IGraphSelectionStrategy
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Maps the property do a named graph selected by a given function
        /// </summary>
		public PredicatePart SelectedBy(Func<EntityId, Uri> createGraphUri)
		{
			_propertyMap.GraphSelector = new FuncGraphSelector(createGraphUri);
			return _predicatePart;
		}
	}
}