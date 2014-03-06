using System;
using RomanticWeb.Entities.ResultAggregations;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Mapping for an Entity's property
    /// </summary>
    public interface IPropertyMapping
	{
        /// <summary>
        /// Gets the RDF predicate URI
        /// </summary>
		Uri Uri { get; }

        /// <summary>
        /// Gets the property name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the property's return type
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Gets the storage strategy for a mapped property
        /// </summary>
        StorageStrategyOption StorageStrategy { get; set; }

        Aggregation? Aggregation { get; set; }
	}
}