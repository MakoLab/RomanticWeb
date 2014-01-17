using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Allows mapping a property to a named graph
    /// </summary>
    public sealed class NamedGraphPart<TParentMap> where TParentMap : INamedGraphSelectingMap
	{
        private readonly TParentMap _parentMap;
		private readonly INamedGraphSelectingMap _propertyMap;

        internal NamedGraphPart(TParentMap parentMap)
		{
			_parentMap = parentMap;
            _propertyMap = parentMap;
		}

        /// <summary>
        /// Sets the implementation of <see cref="GraphSelectionStrategyBase"/>
        /// to use when resolving named graphs for this term
        /// </summary>
        public TParentMap SelectedBy<T>() where T : GraphSelectionStrategyBase, new()
		{
		    _propertyMap.GraphSelector=new T();
		    return _parentMap;
		}

        /// <summary>
        /// Maps the property do a named graph selected by a given function
        /// </summary>
        public TParentMap SelectedBy(Func<EntityId, Uri> createGraphUri)
		{
			_propertyMap.GraphSelector = new FuncGraphSelector(createGraphUri);
			return _parentMap;
		}
	}
}