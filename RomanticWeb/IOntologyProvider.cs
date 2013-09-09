using System.Collections.Generic;

namespace RomanticWeb
{
    public interface IOntologyProvider
    {
        IEnumerable<Ontology> Ontologies { get; }
    }
}