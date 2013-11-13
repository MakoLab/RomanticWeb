using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    public sealed class MappingContext
    {
        private readonly IOntologyProvider _ontologyProvider;

        private readonly IGraphSelectionStrategy _defaultGraphSelector;

        public MappingContext(IOntologyProvider ontologyProvider,IGraphSelectionStrategy defaultGraphSelector)
        {
            _ontologyProvider=ontologyProvider;
            _defaultGraphSelector=defaultGraphSelector;
        }

        public IOntologyProvider OntologyProvider
        {
            get
            {
                return _ontologyProvider;
            }
        }

        public IGraphSelectionStrategy DefaultGraphSelector
        {
            get
            {
                return _defaultGraphSelector;
            }
        }
    }
}