using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    internal interface IObjectAccessor
    {
        dynamic GetObjects(EntityId entity, Property predicate);
    }
}