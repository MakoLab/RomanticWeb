using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// A proxy, which mediates between <see cref="IEntity"/> instances and RDF data
    /// </summary>
    public interface IEntityProxy
    {
        /// <summary>
        /// Gets the graph selection override.
        /// </summary>
        /// <value>
        /// The graph selection override.
        /// </value>
        ISourceGraphSelectionOverride GraphSelectionOverride { get; }

        /// <summary>
        /// Gets the entity's identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        EntityId Id { get; }

        /// <summary>
        /// Gets the entity mapping.
        /// </summary>
        /// <value>
        /// The entity mapping.
        /// </value>
        IEntityMapping EntityMapping { get; }

        /// <summary>
        /// Overrides the graph selection.
        /// </summary>
        /// <param name="graphOverride">The graph override.</param>
        void OverrideGraphSelection(ISourceGraphSelectionOverride graphOverride);
    }
}