using System;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>Defines method for selecting named graph URI based on <see cref="EntityId"/>.</summary>
    public abstract class GraphSelectionStrategyBase : INamedGraphSelector
    {
        /// <inheritdoc />
        Uri INamedGraphSelector.SelectGraph(EntityId entityId, [AllowNull] IEntityMapping entityMapping, [AllowNull] IPropertyMapping predicate)
        {
            var nonBlankId = entityId;
            while (nonBlankId is BlankId)
            {
                nonBlankId = ((BlankId)entityId).RootEntityId;

                if (nonBlankId == null)
                {
                    throw new ArgumentException("Blank node must have parent id", "entityId");
                }
            }

            return GetGraphForEntityId(nonBlankId, entityMapping, predicate);
        }

        /// <summary>Gets a named graph URI for a given entity.</summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entityMapping">The entity mapping.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>In implementing classes returns an absolute <see cref="Uri"/>.</returns>
        protected abstract Uri GetGraphForEntityId(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping predicate);
    }
}