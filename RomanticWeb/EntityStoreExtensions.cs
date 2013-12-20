using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb
{
    internal static class EntityStoreExtensions
    {
        public static bool EntityIsCollectionRoot(this IEntityStore store, IEntity potentialList)
        {
            return !(from propertyObjectPair in store.Quads
                     where propertyObjectPair.Predicate == Node.ForUri(Rdf.rest)
                     && propertyObjectPair.Object == Node.ForUri(potentialList.Id.Uri)
                     select propertyObjectPair).Any();
        }
    }
}