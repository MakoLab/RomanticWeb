using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public interface IParent2
    {
        [Property("magi", "parent2")]
        string Member { get; }
    }
}