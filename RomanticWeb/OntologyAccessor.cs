using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NullGuard;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Allows dynamic resolution of prediacte URIs based dynamic member name and Ontology prefix
    /// </summary>
    [NullGuard(ValidationFlags.OutValues)]
    public sealed class OntologyAccessor : DynamicObject, IObjectAccessor
    {
        private readonly ITriplesSource _tripleSource;
        private readonly EntityId _entityId;
        private readonly Ontology _ontology;
        private readonly IEntityFactory _entityFactory;

        /// <summary>
        /// Creates a new instance of <see cref="OntologyAccessor"/>
        /// </summary>
        /// <param name="tripleSource">underlying RDF source</param>
        /// <param name="entity">the access Entity</param>
        /// <param name="ontology">Ontolgy used to resolve predicate names</param>
        /// <param name="entityFactory">factory used to produce associated Entities</param>
        public OntologyAccessor(ITriplesSource tripleSource, Entity entity, Ontology ontology, IEntityFactory entityFactory)
        {
            _tripleSource = tripleSource;
            _entityId = entity.Id;
            _ontology = ontology;
            _entityFactory = entityFactory;
        }

        internal IEnumerable<Property> KnownProperties
        {
            get { return _ontology.Properties; }
        }

        /// <summary>
        /// Tries to retrieve subjects from the backing RDF source for a dynamically resolved property
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = KnownProperties.SingleOrDefault(p => p.PropertyName == binder.Name);

            if (property == null)
            {
                property = new Property(binder.Name).InOntology(_ontology);
            }

            result = ((IObjectAccessor)this).GetObjects(_entityId, property);

            return true;
        }

        dynamic IObjectAccessor.GetObjects(EntityId entity, Property predicate)
        {
            var subjectValues = _tripleSource.GetObjectsForPredicate(entity, predicate);
            var subjects = (from subject in subjectValues
                            select Convert(subject)).ToList();

            if (subjects.Count == 1)
            {
                return subjects.Single();
            }

            if (subjects.Count == 0)
            {
                return null;
            }

            return subjects;
        }

        private object Convert(RdfNode subject)
        {
            if (subject.IsUri)
            {
                return _entityFactory.Create(new UriId(subject.Uri));
            }
            if (subject.IsBlank)
            {
                return _entityFactory.Create(new BlankId(subject.BlankNodeId, subject.GraphUri));
            }

            return subject.Literal;
        }
    }
}