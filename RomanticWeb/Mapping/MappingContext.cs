using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a mapping context.</summary>
    public sealed class MappingContext
    {
        private readonly IOntologyProvider _ontologyProvider;

        /// <summary>Default constructor with ontology provider and default graph selector passed.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        public MappingContext(IOntologyProvider ontologyProvider)
        {
            _ontologyProvider=ontologyProvider;
        }

        /// <summary>Gets the ontology provider.</summary>
        public IOntologyProvider OntologyProvider { get { return _ontologyProvider; } }
    }
}