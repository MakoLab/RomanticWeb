using VDS.RDF;

namespace RomanticWeb.Tests.Helpers
{
    public static class TestingExtensions
    {
        public static StoreAssertions Should(this IInMemoryQueryableStore store)
        {
            return new StoreAssertions(store);
        }
    }
}