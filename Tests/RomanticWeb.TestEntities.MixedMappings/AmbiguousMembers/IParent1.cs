using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public interface IParent1 : IEntity
    {
        [Property("magi", "parent1")]
        string Member { get; }
    }
}