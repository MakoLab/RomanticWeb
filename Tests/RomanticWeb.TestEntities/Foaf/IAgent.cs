using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Foaf
{
    [Class("foaf","Agent")]
    public interface IAgent:IEntity
    {
        [Property("foaf","knows")]
        IAgent KnowsOne { get; set; }

        [Collection("foaf", "knows")]
        ICollection<IAgent> Knows { get; }

        [Property("foaf","gender")]
        string Gender { get; set; }
    }
}