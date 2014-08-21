using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Defines contract for classes, which aggregate information about mapped RDF types
    /// </summary>
    public interface IRdfTypeCache
    {
        /// <summary>
        /// Gets the type of the most derived mapped.
        /// </summary>
        /// <param name="entityTypes"><see cref="System.Uri "/>s of types of given entity.</param>
        /// <param name="requestedType">Requested <see cref="IEntity"/> type.</param>
        /// <returns>the <paramref name="requestedType"/> or a type derived from it</returns>
        IEnumerable<Type> GetMostDerivedMappedTypes(IEnumerable<Uri> entityTypes, Type requestedType);

        void Add(Type entityType, IList<IClassMapping> classMappings);
    }
}