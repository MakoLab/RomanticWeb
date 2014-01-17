using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Helper methods for configuring <see cref="EntityContextFactory"/>
    /// </summary>
    public static class EntityContextFactoryExtensions
    {
        /// <summary>Includes default <see cref="IOntologyProvider" />s in context that will be created.</summary>
        /// <returns>The <see cref="EntityContextFactory" /> </returns>
        public static EntityContextFactory WithDefaultOntologies(this EntityContextFactory factory)
        {
            return factory.WithOntology(new DefaultOntologiesProvider());
        }
    }
}