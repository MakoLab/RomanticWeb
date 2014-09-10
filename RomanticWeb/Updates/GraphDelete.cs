using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change, which deletes a named graph
    /// </summary>
    public class GraphDelete : DatasetChange
    {
        internal GraphDelete(EntityId entity, EntityId graph)
            : base(entity, graph)
        {
        }
    }
}