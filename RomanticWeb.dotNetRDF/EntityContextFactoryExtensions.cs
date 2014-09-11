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
    }
}