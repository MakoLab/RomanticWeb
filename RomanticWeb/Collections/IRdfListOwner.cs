using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Collections
{
    /// <summary>
    /// Represents an entity, which has a relation to
    /// </summary>
    public interface IRdfListOwner : IEntity
    {
        /// <summary>
        /// Gets the first node of the rdf:List.
        /// </summary>
        [Property(Rdf.BaseUri + "object")]
        IEntity ListHead { get; set; }
    }
}