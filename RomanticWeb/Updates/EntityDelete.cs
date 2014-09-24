using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change, which deletes an entity
    /// </summary>
    public class EntityDelete : DatasetChange
    {
        internal EntityDelete(EntityId entity)
            : base(entity)
        {
        }

        /// <summary>
        /// Returns a description of the change
        /// </summary>
        public override string ToString()
        {
            return "Delete entity " + Entity;
        }

        /// <inheritdoc />
        public override bool CanMergeWith(DatasetChange other)
        {
            return false;
        }

        /// <inheritdoc />
        public override DatasetChange MergeWith(DatasetChange other)
        {
            throw new InvalidOperationException("Cannot merge EntityDelete with any other change");
        }
    }
}