using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Implementation of <see cref="IGraphSelectionStrategy"/>, 
    /// which allows the use of any arbitrary function
    /// to select named graph URI based on <see cref="EntityId"/>
    /// </summary>
	public class FuncGraphSelector : IGraphSelectionStrategy
	{
		private readonly Func<EntityId, Uri> _createGraphUri;

        internal FuncGraphSelector(Func<EntityId, Uri> createGraphUri)
		{
			_createGraphUri = createGraphUri;
		}

        /// <inheritdoc />
		public Uri SelectGraph(EntityId entityId)
		{
			return _createGraphUri(entityId);
		}
	}
}