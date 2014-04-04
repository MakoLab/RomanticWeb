using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RomanticWeb.Ontologies
{
    /// <summary>Converts an OWL based ontology written with XML syntax into an object representation.</summary>
    public class XmlOntologyFactory:IOntologyFactory
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
            bool isOwlBasedFile=true;
            XDocument document=XDocument.Load(fileStream);
            XElement ontologyElement=(from element in document.Descendants() where element.Name.LocalName=="Ontology" select element).FirstOrDefault();
            if (ontologyElement==null)
            {
                isOwlBasedFile=false;
                ontologyElement=(from element in document.Descendants() where element.Name.LocalName=="Description" select element).FirstOrDefault();
                if (ontologyElement==null)
                {
                    throw new ArgumentOutOfRangeException("Provided stream does not contain ontology information suitable for usage.");
                }
            }

            NamespaceSpecification namespaceSpecification=null;
            string displayName=null;
            IEnumerable<Term> terms=null;
            namespaceSpecification=(
                from attribute in ontologyElement.Attributes()
                where attribute.Name.LocalName=="about"
                select new NamespaceSpecification(ontologyElement.GetPrefixOfNamespace(attribute.Value),attribute.Value)).FirstOrDefault();
            displayName=(
                from child in ontologyElement.Descendants()
                where (child.Name.LocalName=="label")||(child.Name.LocalName=="title")
                select child.Value).FirstOrDefault();
            if (isOwlBasedFile)
            {
                terms=CreateFromOWLXML(document,namespaceSpecification.BaseUri);
            }
            else
            {
                terms=CreateFromRDFXML(document,namespaceSpecification.BaseUri);
            }

            return new Ontology(displayName,namespaceSpecification,terms.ToArray());
        }

        private IEnumerable<Term> CreateFromOWLXML(XDocument document,Uri baseUri)
        {
            return (from element in document.Descendants()
                    where AcceptedNodeTypes.Contains(element.Name.LocalName)
                    from attribute in element.Attributes()
                    where (attribute.Name.LocalName=="about")&&(attribute.Value.StartsWith(baseUri.AbsoluteUri))
                    select CreateTerm(element.Name.LocalName,attribute.Value.Substring(baseUri.AbsoluteUri.Length)));
        }

        private IEnumerable<Term> CreateFromRDFXML(XDocument document,Uri baseUri)
        {
            return (from element in document.Descendants()
                    where (element.Name.LocalName=="Description")
                    from child in element.Descendants()
                    where (child.Name.LocalName=="type")
                    from childAttribute in child.Attributes()
                    where (childAttribute.Name.LocalName=="resource")&&(AcceptedNodeTypes.Any(nodeName => childAttribute.Value.EndsWith(nodeName)))
                    from attribute in element.Attributes()
                    where (attribute.Name.LocalName=="about")&&(attribute.Value.StartsWith(baseUri.AbsoluteUri))
                    let typeName=AcceptedNodeTypes.First(nodeName => childAttribute.Value.EndsWith(nodeName))
                    select CreateTerm(typeName,attribute.Value.Substring(baseUri.AbsoluteUri.Length)));
        }

        private Term CreateTerm(string typeName,string termName)
        {
            switch (typeName)
            {
                case "Class":
                    return new RomanticWeb.Ontologies.Class(termName);
                case "Property":
                    return new RomanticWeb.Ontologies.Property(termName);
                case "DatatypeProperty":
                    return new RomanticWeb.Ontologies.DatatypeProperty(termName);
                case "ObjectProperty":
                    return new RomanticWeb.Ontologies.ObjectProperty(termName);
                default:
                    throw new ArgumentNullException("typeName");
            }
        }
    }
}