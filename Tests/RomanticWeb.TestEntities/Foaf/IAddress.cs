using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    public interface IAddress : IEntity
    {
        [Property("http://schema.org/addressLocality")]
        string City { get; set; }

        [Property("http://schema.org/streetAddress")]
        string Street { get; set; }
    }
}