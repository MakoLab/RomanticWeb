using RomanticWeb.ComponentModel;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>
    /// </summary>
    public static class EntityContextFactoryExtensions
    {
        /// <summary>
        /// Sets up the <paramref name="factory"/> with components required to use dotNetRDF
        /// </summary>
        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory)
        {
            return factory.WithDependencies<Components>()
                          .WithEntitySource<TripleStoreAdapter>();
        }

        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory, ITripleStore store)
        {
            ((IComponentRegistryFacade)factory).Register(store);
            return WithDotNetRDF(factory);
        }

        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory, string storeName)
        {
            ((IComponentRegistryFacade)factory).Register(Configuration.StoresConfigurationSection.Default.CreateStore(storeName));
            return WithDotNetRDF(factory);
        }
    }
}