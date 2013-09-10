using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Dynamic;

namespace RomanticWeb
{
    /// <summary>
    /// An RDF entity, which can be used to dynamically access RDF triples
    /// </summary>
    public class Entity : ImpromptuDictionary
    {
        private readonly EntityId _entityId;

        /// <summary>
        /// Creates a new instance of <see cref="Entity"/>
        /// </summary>
        /// <remarks>It will not be backed by <b>any</b> triples, when not created via factory</remarks>
        public Entity(EntityId entityId)
        {
            _entityId = entityId;
        }

        /// <summary>
        /// Gets the entity's identifier
        /// </summary>
        protected internal EntityId Id
        {
            get { return _entityId; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // first look for ontology prefix
            bool gettingMemberSucceeded = base.TryGetMember(binder, out result);

            if (gettingMemberSucceeded)
            {
                return true;
            }

            // then look for properties in ontologies
            if (TryGetPropertyFromOntologies(binder, out result))
            {
                return true;
            }

            throw new UnknownNamespaceException(binder.Name);
        }

        private bool TryGetPropertyFromOntologies(GetMemberBinder binder, out object result)
        {
            var matchingPredicates = (from accessor in Values.OfType<IPredicateAccessor>()
                                      from property in accessor.Ontology.Properties
                                      where property.PredicateName == binder.Name
                                      select new
                                          {
                                              accessor,
                                              property
                                          }).ToList();

            if (matchingPredicates.Count == 1)
            {
                var singleMatch = matchingPredicates.Single();
                result = singleMatch.accessor.GetObjects(singleMatch.property);
                return true;
            }

            if (matchingPredicates.Count == 0)
            {
                result = null;
                return false;
            }

            var matchedPropertiesQNames = matchingPredicates.Select(pair => pair.accessor.Ontology.Prefix);
            throw new AmbiguousPropertyException(binder.Name, matchedPropertiesQNames);
        }
    }
}
