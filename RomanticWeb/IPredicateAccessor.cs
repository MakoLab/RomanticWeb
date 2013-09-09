using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetObjects(Uri baseUri, Property predicate);
    }
}