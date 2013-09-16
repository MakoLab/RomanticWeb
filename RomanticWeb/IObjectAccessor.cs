using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public interface IObjectAccessor
	{
		dynamic GetObjects(EntityId entity, Property predicate);
	}
}