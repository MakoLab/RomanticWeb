using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public class GraphDelete : DatasetChange
    {
        internal GraphDelete(EntityId changedEntity, EntityId graph)
            : base(changedEntity, graph)
        {
        }
    }
}