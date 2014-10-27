using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public interface IChild : IParent1, IParent2, IParent3
    {
        [Property("magi", "child")]
        decimal ChildMember { get; }
    }
}