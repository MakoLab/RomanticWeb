using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public class ChildMap : EntityMap<IChild>
    {
        public ChildMap()
        {
            Property(p => p.ChildMember).Term.Is("magi", "child");
        }
    }
}