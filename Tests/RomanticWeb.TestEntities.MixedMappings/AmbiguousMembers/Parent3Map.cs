using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public class Parent3Map : EntityMap<IParent3>
    {
        public Parent3Map()
        {
            Property(p => p.Member).Term.Is("magi", "parent3");
        }
    }
}