using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>A highest level of entity mapping, used to access property mappings, type mappings, etc.</summary>
	public interface IEntityMapping
	{
        /// <summary>Gets the type of the mapped entity.</summary>
        Type EntityType { get; }

        /// <summary>Gets the RDF type mapping.</summary>
		IEnumerable<IClassMapping> Classes { get; }

        /// <summary>Gets the RDF type mapping.</summary>
        IEnumerable<IPropertyMapping> Properties { get; }

        /// <summary>Gets the property mapping for a property by name.</summary>
		IPropertyMapping PropertyFor(string propertyName);
	}
}