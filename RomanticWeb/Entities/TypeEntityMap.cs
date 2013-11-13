using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Entities
{
    internal class TypeEntityMap:EntityMap<ITypedEntity>
    {
        public TypeEntityMap(Uri graphUri)
        {
            Collection(typed => typed.Types)
                .Predicate.Is("rdf", "type")
                .NamedGraph.SelectedBy(id => graphUri);
        }
    }
}