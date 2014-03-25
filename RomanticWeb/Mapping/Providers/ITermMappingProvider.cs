using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// A <see cref="IMappingProvider"/>, which provides a term mapping
    /// </summary>
    public interface ITermMappingProvider:IMappingProvider
    {
        /// <summary>
        /// Gets or sets the factory method for resolving the mapped term.
        /// </summary>
        Func<IOntologyProvider,Uri> GetTerm { get; set; }
    }
}