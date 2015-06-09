using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.NamedGraphs
{
    /// <summary>Defines contract for selecting or creating a named graph URI.</summary>
    public interface INamedGraphSelector
    {
        /// <summary>Selects the names graph URI.</summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entityMapping">The entity mapping.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>In implementing classes returns an absolute <see cref="Uri"/>.</returns>
        Uri SelectGraph(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping predicate);
    }
}