using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    // todo: give a more descriptive name
    public sealed class MappingContext
    {
        private readonly IOntologyProvider _ontologyProvider;

        private readonly IGraphSelectionStrategy _defaultGraphSelector;

        public MappingContext(IOntologyProvider ontologyProvider,IGraphSelectionStrategy defaultGraphSelector)
        {
            _ontologyProvider=ontologyProvider;
            _defaultGraphSelector=defaultGraphSelector;
        }

        /// <summary>
        /// Gets the ontology provider
        /// </summary>
        public IOntologyProvider OntologyProvider
        {
            get
            {
                return _ontologyProvider;
            }
        }

        /// <summary>
        /// Gets the default <see cref="IGraphSelectionStrategy"/>, which is used as fallback
        /// </summary>
        public IGraphSelectionStrategy DefaultGraphSelector
        {
            get
            {
                return _defaultGraphSelector;
            }
        }
    }
}