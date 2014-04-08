using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    [Class("foaf","Person")]
    public interface IAlsoPerson:IAgent
    {
        [Property("foaf", "familyName")]
        string LastName { get; }
    }
}