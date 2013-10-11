using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    public class OntologyProviderBase:IOntologyProvider
    {
        public OntologyProviderBase()
        {
            Ontologies=new Ontology[0];
        }

        public OntologyProviderBase(IEnumerable<Ontology> ontologies)
        {
            Ontologies=ontologies;
        }

        public virtual IEnumerable<Ontology> Ontologies { get; private set; }

        public virtual Uri ResolveUri(string prefix, string rdfTermName)
        {
            return (from ontology in this.Ontologies
                    where ontology.Prefix == prefix
                    select new Uri(ontology.BaseUri + rdfTermName)).Single();
        }
    }
}