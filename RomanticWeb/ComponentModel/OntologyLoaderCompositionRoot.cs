using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Validation;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.ComponentModel
{
    internal sealed class OntologyLoaderCompositionRoot : CompositionRootBase
    {
        public OntologyLoaderCompositionRoot()
        {
            OntologyLoader<XmlOntologyLoader>();
        }
    }
}