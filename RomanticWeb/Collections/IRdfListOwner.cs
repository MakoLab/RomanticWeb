using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Collections
{
    public interface IRdfListOwner : IEntity
    {
        [Property(Rdf.BaseUri + "object")]
        IEntity ListHead { get; set; }
    }
}