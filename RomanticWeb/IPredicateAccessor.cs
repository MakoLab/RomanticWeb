using System;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetObjects(Uri baseUri, Property predicate);
    }
}