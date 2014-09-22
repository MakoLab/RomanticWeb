using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    public interface INoTypeAgent : IEntity
    {
        [Collection("foaf", "knows")]
        IList<INoTypeAgent> Knows { get; }

        [Collection("foaf", "knows")]
        ICollection<INoTypeAgent> KnowsCollection { get; }

        [Property("foaf", "givenName")]
        string Name { get; set; }
    }
}