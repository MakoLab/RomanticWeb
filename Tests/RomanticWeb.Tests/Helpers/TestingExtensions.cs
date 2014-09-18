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