using RomanticWeb.Ontologies;
using RomanticWeb.dotNetRDF.TripleSources;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    /// <summary>
    /// Entity factory implementation backed by a triple store
    /// </summary>
    public class EntityFactory : RomanticWeb.EntityFactoryBase
    {
        private readonly ITripleStore _tripeStore;

        /// <summary>
        /// Creates a new instance of <see cref="EntityFactory"/>
        /// </summary>
        /// <param name="tripeStore">Triples will be read from this triple store</param>
        /// <param name="ontologyProvider">Entites will expose dynamic members for the provided ontologies</param>
        public EntityFactory(ITripleStore tripeStore, IMappingProvider mappings, IOntologyProvider ontologyProvider)
            : base(mappings, ontologyProvider)
        {
            _tripeStore = tripeStore;
        }

        protected override ITriplesSource CreateTriplesSourceForEntity<TEntity>(IMapping<TEntity> mappingFor)
        {
            return new UnionGraphSource(_tripeStore);
        }

        protected override ITriplesSource CreateTriplesSourceForOntology()
        {
            return new UnionGraphSource(_tripeStore);
        }
    }
}
