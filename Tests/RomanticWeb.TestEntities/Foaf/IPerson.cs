using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    [Class("foaf", "Person")]
    public interface IPerson : IAgent
    {
        [Property("foaf", "givenName")]
        string Name { get; set; }
    }
}