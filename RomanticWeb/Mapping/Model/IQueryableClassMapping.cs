using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// A <see cref="IClassMapping"/>, which affects how an entity is queried
    /// </summary>
    public interface IQueryableClassMapping:IClassMapping
    {
        /// <summary>
        /// Gets the URIs to query for.
        /// </summary>
        IEnumerable<Uri> Uris { get; } 
    }
}