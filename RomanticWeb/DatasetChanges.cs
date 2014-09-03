using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>Represents changes made in the triple store.</summary>
    public sealed class DatasetChanges
    {
        internal DatasetChanges(
            IEnumerable<EntityQuad> quadsAdded,
            IEnumerable<EntityQuad> quadsRemoved,
            IEnumerable<EntityQuad> entitiesReconstructed,
            IEnumerable<EntityId> entitiesRemoved)
        {
            QuadsAdded = quadsAdded;
            QuadsRemoved = quadsRemoved;
            EntitiesReconstructed = entitiesReconstructed;
            EntitiesRemoved = entitiesRemoved;
        }

        internal DatasetChanges()
            : this(new EntityQuad[0], new EntityQuad[0], new EntityQuad[0], new EntityId[0])
        {
        }

        /// <summary>Gets the added quads.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to insert these statements after quads are removed.</remarks>
        public IEnumerable<EntityQuad> QuadsAdded { get; private set; }

        /// <summary>Gets the quads removed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are to delete these statements first.</remarks>
        public IEnumerable<EntityQuad> QuadsRemoved { get; private set; }

        /// <summary>Gets the entity graphs to be reconstructed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to delete whole graph of a given entity, and then to insert all given statements.</remarks>
        public IEnumerable<EntityQuad> EntitiesReconstructed { get; private set; }

        /// <summary>Gets the entities to be removed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to delete whole graph of a given entity.</remarks>
        public IEnumerable<EntityId> EntitiesRemoved { get; private set; }

        /// <summary>Gets a value indicating whether there are any changes.</summary>
        public bool Any { get { return (QuadsAdded.Any()) || (QuadsRemoved.Any()) || (EntitiesReconstructed.Any()) || (EntitiesRemoved.Any()); } }
    }
}