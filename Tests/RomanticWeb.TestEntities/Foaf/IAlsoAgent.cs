using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    [Class("foaf","Agent")]
    public interface IAlsoAgent:IEntity
    {
        [Collection("foaf", "knows")]
        IList<IAgent> Knows { get; }

        [Property("foaf","gender")]
        string Gender { get; set; }
    }
}