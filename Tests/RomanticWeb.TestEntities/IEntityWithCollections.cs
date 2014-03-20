using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.TestEntities
{
    public interface IEntityWithCollections:IEntity
    {
        [Collection("magi","collection")]
        IList<string> DefaultListMapping { get; set; }

        [Collection("magi","collection")]
        IEnumerable<string> DefaultEnumerableMapping { get; set; }

        [Collection("magi","collection")]
        ICollection<string> DefaultCollectionMapping { get; set; }

        [Collection("magi","collection",StoreAs=StoreAs.SimpleCollection)]
        IList<string> OverridenListMapping { get; set; }

        [Collection("magi","collection",StoreAs=StoreAs.RdfList)]
        IEnumerable<string> OverridenEnumerableMapping { get; set; }

        [Collection("magi","collection",StoreAs=StoreAs.RdfList)]
        ICollection<string> OverridenCollectionMapping { get; set; }
    }
}