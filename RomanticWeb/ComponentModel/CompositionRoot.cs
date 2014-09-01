using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb.ComponentModel
{
    internal sealed class CompositionRoot : CompositionRootBase
    {
        public CompositionRoot()
        {
            EntityContext<EntityContext>();
            BlankNodeIdGenerator<DefaultBlankNodeIdGenerator>();
            Ontology<DefaultOntologiesProvider>();
            NamedGraphSelector<NamedGraphSelector>();
            EntityStore<EntityStore>();
            FallbackNodeConverter<FallbackNodeConverter>();
        }
    }
}