using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetObjects(Property predicate);

        Ontology Ontology { get; }
    }
}