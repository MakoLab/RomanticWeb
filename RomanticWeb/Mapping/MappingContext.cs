using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a mapping context.</summary>
    public sealed class MappingContext
    {
        private readonly IOntologyProvider _ontologyProvider;
        private readonly GraphSelectionStrategyBase _defaultGraphSelector;

        /// <summary>Default constructor with ontology provider and default graph selector passed.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="defaultGraphSelector">Default graph selector.</param>
        public MappingContext(IOntologyProvider ontologyProvider,GraphSelectionStrategyBase defaultGraphSelector)
        {
            _ontologyProvider=ontologyProvider;
            _defaultGraphSelector=defaultGraphSelector;
        }

        /// <summary>Gets the ontology provider.</summary>
        public IOntologyProvider OntologyProvider { get { return _ontologyProvider; } }

        /// <summary>Gets the default <see cref="GraphSelectionStrategyBase"/>, which is used as fallback.</summary>
        public GraphSelectionStrategyBase DefaultGraphSelector { get { return _defaultGraphSelector; } }
    }
}