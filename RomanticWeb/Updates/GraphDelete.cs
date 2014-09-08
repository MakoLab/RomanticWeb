using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    internal class GraphDelete : DatasetChange
    {
        public GraphDelete(EntityId changedEntity, EntityId graph)
            : base(changedEntity, graph)
        {
        }
    }
}