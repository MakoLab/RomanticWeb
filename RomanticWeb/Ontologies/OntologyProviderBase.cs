using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a base behavior for ontology providers.</summary>
    public class OntologyProviderBase:IOntologyProvider
    {
        /// <summary>Default parameterles constructor.</summary>
        public OntologyProviderBase()
        {
            Ontologies=new Ontology[0];
        }

        /// <summary>Constructor with an enumeration of ontologies to be included</summary>
        /// <param name="ontologies"></param>
        public OntologyProviderBase(IEnumerable<Ontology> ontologies)
        {
            Ontologies=ontologies;
        }

        /// <summary>Get ontologies' metadata.</summary>
        public virtual IEnumerable<Ontology> Ontologies { get; private set; }

        /// <summary>Gets a URI from a QName.</summary>
        [return: AllowNull]
        public virtual Uri ResolveUri(string prefix,string rdfTermName)
        {
            return Ontologies.Where(ontology => ontology.Prefix==prefix).Select(ontology => new Uri(ontology.BaseUri+rdfTermName)).FirstOrDefault();
        }
    }
}