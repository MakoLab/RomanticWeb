using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// A RDF type mapping for an Entity
    /// </summary>
    public interface IClassMapping
    {
        bool IsInherited { get; }

        bool IsMatch(IEnumerable<Uri> classList);

        void AppendTo(ICollection<EntityId> classList);

        /// <summary>
        /// Accepts the specified mapping model visitor.
        /// </summary>
        /// <param name="mappingModelVisitor">The mapping model visitor.</param>
        void Accept(IMappingModelVisitor mappingModelVisitor);
	}
}