using System;
using RomanticWeb.Mapping.Visitors;

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
        /// Accepts the specified mapping model visitor.
        /// </summary>
        /// <param name="mappingModelVisitor">The mapping model visitor.</param>
        void Accept(IMappingModelVisitor mappingModelVisitor);
	}
}