using System.Collections.Generic;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class EntityWithCollectionsMap:EntityMap<IEntityWithCollections>
    {
        public EntityWithCollectionsMap()
        {
            foreach (var collectionMap in Collections)
            {
                collectionMap.Term.Is("magi","collection");
            }
        }

        private IEnumerable<ICollectionMap> Collections
        {
            get
            {
                yield return Collection(e => e.DefaultListMapping);
                yield return Collection(e => e.DefaultCollectionMapping);
                yield return Collection(e => e.DefaultEnumerableMapping);
                yield return Collection(e => e.OverridenCollectionMapping).StoreAs.RdfList();
                yield return Collection(e => e.OverridenEnumerableMapping).StoreAs.RdfList();
                yield return Collection(e => e.OverridenListMapping).StoreAs.SimpleCollection();
            }
        }
    }
}