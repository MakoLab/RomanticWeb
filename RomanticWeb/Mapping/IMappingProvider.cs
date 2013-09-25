using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Defines methods for classes which build entity mappings
    /// </summary>
    public interface IMappingProvider
	{
        /// <summary>
        /// Creates a mapping for a single entity
        /// </summary>
        IEntityMapping CreateMapping(IOntologyProvider prefixes);
	}
}