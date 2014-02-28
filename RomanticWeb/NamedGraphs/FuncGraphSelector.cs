using System;
using RomanticWeb.Entities;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>
    /// Implementation of <see cref="GraphSelectionStrategyBase"/>, 
    /// which allows the use of any arbitrary function
    /// to select named graph URI based on <see cref="EntityId"/>
    /// </summary>
	public class FuncGraphSelector : GraphSelectionStrategyBase
	{
		private readonly Func<EntityId, Uri> _createGraphUri;

        internal FuncGraphSelector(Func<EntityId, Uri> createGraphUri)
		{
			_createGraphUri = createGraphUri;
		}

        /// <inheritdoc />
        protected override Uri GetGraphForEntityId(EntityId entityId,Uri predicate)
		{
			return _createGraphUri(entityId);
		}
	}
}