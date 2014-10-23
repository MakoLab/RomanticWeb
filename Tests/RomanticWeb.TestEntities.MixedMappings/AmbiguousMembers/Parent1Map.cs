using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers
{
    public class Parent1Map : EntityMap<IParent1>
    {
        public Parent1Map()
        {
            Property(p => p.Member).Term.Is("magi", "parent1");
        }
    }
}