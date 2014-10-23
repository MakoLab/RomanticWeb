using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public class Parent2Map : EntityMap<IParent2>
    {
        public Parent2Map()
        {
            Property(p => p.Member).Term.Is("magi", "parent2");
        }
    }
}