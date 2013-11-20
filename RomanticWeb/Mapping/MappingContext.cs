using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    // todo: give a more descriptive name

    /// <summary>Provides a mapping context.</summary>
    public sealed class MappingContext
    {
        private readonly IOntologyProvider _ontologyProvider;
        private readonly IGraphSelectionStrategy _defaultGraphSelector;

        /// <summary>Default constructor with ontology provider and default graph selector passed.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="defaultGraphSelector">Default graph selector.</param>
        public MappingContext(IOntologyProvider ontologyProvider,IGraphSelectionStrategy defaultGraphSelector)
        {
            _ontologyProvider=ontologyProvider;
            _defaultGraphSelector=defaultGraphSelector;
        }

        /// <summary>Gets the ontology provider.</summary>
        public IOntologyProvider OntologyProvider { get { return _ontologyProvider; } }

        /// <summary>Gets the default <see cref="IGraphSelectionStrategy"/>, which is used as fallback.</summary>
        public IGraphSelectionStrategy DefaultGraphSelector { get { return _defaultGraphSelector; } }
    }
}