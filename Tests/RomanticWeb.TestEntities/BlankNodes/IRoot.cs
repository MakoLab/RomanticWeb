using RomanticWeb.Entities;

namespace RomanticWeb.TestEntities.BlankNodes
{
    public interface IRoot:IEntity
    {
        INested Child { get; }
    }
}