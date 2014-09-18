using NullGuard;
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
        /// Initializes a new instance of the <see cref="DatasetChange"/> class, which affects a single graph.
        /// </summary>
        /// <param name="entity">The changed entity.</param>
        /// <param name="graph">The changed graph.</param>
        protected DatasetChange(EntityId entity, EntityId graph)
            : this(entity)
        {
            _graph = graph;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetChange"/> class, which affects multiple graphs.
        /// </summary>
        /// <param name="entity">The changed entity.</param>
        protected DatasetChange(EntityId entity)
        {
            _entity = entity;
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
        /// <returns>null if change affects multiple graphs</returns>
        public EntityId Graph
        {
            [return: AllowNull]
            get
            {
                return _graph;
            }
        }
    }
}