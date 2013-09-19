using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    public abstract class OntologyProviderBase:IOntologyProvider
    {
        public abstract IEnumerable<Ontology> Ontologies { get; }

        public virtual Uri ResolveUri(string prefix, string rdfTermName)
        {
            return (from ontology in this.Ontologies
                    where ontology.Prefix == prefix
                    select new Uri(ontology.BaseUri + rdfTermName)).Single();
        }
    }
}