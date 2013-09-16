using System;

namespace RomanticWeb.Mapping
{
    public class FuncGraphSelector : IGraphSelectionStrategy
    {
        private readonly Func<EntityId, Uri> _createGraphUri;

        public FuncGraphSelector(Func<EntityId, Uri> createGraphUri)
        {
            _createGraphUri = createGraphUri;
        }

        public Uri SelectGraph(EntityId entityId)
        {
            return _createGraphUri(entityId);
        }
    }
}