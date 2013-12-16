using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace RomanticWeb.Ontologies
{
    /// <summary>Converts an OWL based ontology written with XML syntax into an object representation.</summary>
    public class OwlOntologyFactory:IOntologyFactory
    {
        private static readonly string[] AcceptedMimeTypes=new string[] { "application/rdf+xml","application/owl+xml" };
        private static readonly string[] AcceptedNodeTypes=new string[] { "Class","Property","DatatypeProperty","ObjectProperty" };

        /// <summary>Returns a list of accepted content MIME types handled by this factory.</summary>
        public string[] Accepts { get { return AcceptedMimeTypes; } }

        /// <summary>Creates an ontology from given stream.</summary>
        /// <param name="fileStream"></param>
        /// <returns>Ontology filled with terms.</returns>
        public Ontology Create(Stream fileStream)
        {
            return CreateFromXML(fileStream);
        }

        private Ontology CreateFromXML(Stream fileStream)
        {
            NamespaceSpecification namespaceSpecification=null;
            string displayName=null;
            XDocument document=XDocument.Load(fileStream);
            XElement ontologyElement=(from element in document.Descendants() where element.Name.LocalName=="Ontology" select element).FirstOrDefault();
            if (ontologyElement!=null)
            {
                namespaceSpecification=(from attribute in ontologyElement.Attributes()
                                        where attribute.Name.LocalName=="about"
                                        select new NamespaceSpecification(ontologyElement.GetPrefixOfNamespace(attribute.Value),attribute.Value)).FirstOrDefault();
                displayName=(from child in ontologyElement.Descendants()
                             where (child.Name.LocalName=="label")||(child.Name.LocalName=="title")
                             select child.Value).FirstOrDefault();
            }

            IEnumerable<Term> terms=(from element in document.Descendants()
                                     where AcceptedNodeTypes.Contains(element.Name.LocalName)
                                     from attribute in element.Attributes()
                                     where (attribute.Name.LocalName=="about")&&(attribute.Value.StartsWith(namespaceSpecification.BaseUri.AbsoluteUri))
                                     select (Term)Type.GetType(
                                        System.String.Format("RomanticWeb.Ontologies.{0}, RomanticWeb",element.Name.LocalName)).GetConstructor(
                                            BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new Type[] { typeof(string) },null).Invoke(
                                            new object[] { attribute.Value.Substring(namespaceSpecification.BaseUri.AbsoluteUri.Length) }));
            return new Ontology(displayName,namespaceSpecification,terms.ToArray());
        }
    }
}