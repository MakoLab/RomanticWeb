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