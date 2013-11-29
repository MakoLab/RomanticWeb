using System;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// A RDF type mapping for an Entity
    /// </summary>
    public interface IClassMapping
	{
        /// <summary>
        /// Gets the Entity's RDF class URI
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the strategy for selecting named graph URIs
        /// </summary>
        IGraphSelectionStrategy GraphSelector { get; }
	}
}