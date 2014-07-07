using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// A <see cref="IMappingProvider"/>, which provides an entity mapping
    /// </summary>
    public interface IEntityMappingProvider : IMappingProvider
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets the class mapping providers.
        /// </summary>
        IEnumerable<IClassMappingProvider> Classes { get; }

        /// <summary>
        /// Gets the properties mapping providers.
        /// </summary>
        IEnumerable<IPropertyMappingProvider> Properties { get; }
    }
}