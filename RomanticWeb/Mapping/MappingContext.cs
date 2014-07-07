using System.Collections.Generic;
using RomanticWeb.Mapping.Conventions;
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
            : this(ontologyProvider, new IConvention[0])
        {
        }

        /// <summary>Default constructor with ontology provider and default graph selector passed.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="conventions"></param>
        public MappingContext(IOntologyProvider ontologyProvider, IEnumerable<IConvention> conventions)
        {
            _ontologyProvider = ontologyProvider;
            Conventions = conventions;
        }

        /// <summary>Gets the ontology provider.</summary>
        public IOntologyProvider OntologyProvider { get { return _ontologyProvider; } }

        /// <summary>
        /// Gets the conventions.
        /// </summary>
        public IEnumerable<IConvention> Conventions { get; private set; }
    }
}