using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetObjects(Property predicate);
        IEnumerable<Property> KnownProperties { get; }
    }
}