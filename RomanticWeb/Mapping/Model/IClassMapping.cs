using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// A RDF type mapping for entity's RDF class
    /// </summary>
    public interface IClassMapping
    {
        /// <summary>
        /// Gets the RDF class URI.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets a value indicating whether [is inherited] from base class' mapping.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is inherited]; otherwise, <c>false</c>.
        /// </value>
        bool IsInherited { get; }

        /// <summary>
        /// Determines whether the specified class list is match.
        /// </summary>
        /// <param name="classList">The class list.</param>
        /// <returns>true is class list indicates class membership</returns>
        bool IsMatch(IEnumerable<Uri> classList);

        /// <summary>
        /// Accepts the specified mapping model visitor.
        /// </summary>
        /// <param name="mappingModelVisitor">The mapping model visitor.</param>
        void Accept(IMappingModelVisitor mappingModelVisitor);
	}
}