using System;
using VDS.RDF;

namespace RomanticWeb.Tests.Helpers
{
    public static class TestingExtensions
    {
        public static StoreAssertions Should(this IInMemoryQueryableStore store)
        {
            return new StoreAssertions(store);
        }

        public static string Serialize(this IEntityStore entityStore)
        {
            return String.Join(Environment.NewLine, entityStore.Quads);
        } 

        public static StoreAssertions Should(this ITripleStore store)
        {
            var queryableStore = store as IInMemoryQueryableStore;
            if (queryableStore != null)
            {
                return new StoreAssertions(queryableStore);
            }

            throw new NotImplementedException();
        }
    }
}