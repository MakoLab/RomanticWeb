using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public interface IParent1
    {
        [Property("magi", "parent1")]
        string Member { get; }
    }
}