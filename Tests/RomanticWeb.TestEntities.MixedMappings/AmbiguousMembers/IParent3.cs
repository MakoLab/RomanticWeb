using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public interface IParent3
    {
        [Property("magi", "parent3")]
        int Member { get; }
    }
}