using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change to the triple store
    /// </summary>
    public abstract class DatasetChange
    {
        private readonly EntityId _entity;
        private readonly EntityId _graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetChange"/> class.
        /// </summary>
        /// <param name="entity">The changed entity.</param>
        /// <param name="graph">The changed graph.</param>
        protected DatasetChange(EntityId entity, EntityId graph)
        {
            _entity = entity;
            _graph = graph;
        }

        /// <summary>
        /// Gets the entity, which was changed.
        /// </summary>
        public EntityId Entity
        {
            get
            {
                return _entity;
            }
        }

        /// <summary>
        /// Gets the graph, which was changed.
        /// </summary>
        public EntityId Graph
        {
            get
            {
                return _graph;
            }
        }
    }
}