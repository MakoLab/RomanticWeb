using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IMapping<TEntity>
    {
        Property PropertyFor(string propertyName);
    }
}