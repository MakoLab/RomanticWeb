using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public static class EntityContextFactoryExtensions
    {
         public static EntityContextFactory WithDefaultOntologies(this EntityContextFactory factory)
         {
             return factory.WithOntology(new DefaultOntologiesProvider());
         }
    }
}