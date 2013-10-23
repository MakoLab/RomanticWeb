using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Anotar.NLog;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb
{
    public static class EntityStoreExtensions
    {
        public static bool EntityIsCollectionRoot(this IEntityStore store, IEntity potentialList)
        {
            return !(from propertyObjectPair in store.Quads
                     where propertyObjectPair.Predicate == Node.ForUri(Rdf.Rest)
                     && propertyObjectPair.Object == Node.ForUri(potentialList.Id.Uri)
                     select propertyObjectPair).Any();
        }
    }
}