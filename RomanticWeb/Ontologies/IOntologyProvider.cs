using System.Collections.Generic;

namespace RomanticWeb.Ontologies
{
    public interface IOntologyProvider
    {
        IEnumerable<Ontology> Ontologies { get; }
    }
}