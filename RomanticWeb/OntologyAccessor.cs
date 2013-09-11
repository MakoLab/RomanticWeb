using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
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
            var subjects = Convert(subjectValues).ToList();

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

        // todo: refactor this functionality to a specialized class (multiple implementations stored in a lookup dictionary?)
        private IEnumerable<object> Convert(IEnumerable<RdfNode> subjects)
        {
            foreach (var subject in subjects)
            {
                if (subject.IsUri)
                {
                    yield return _entityFactory.Create(new UriId(subject.Uri));
                }
                else if (subject.IsBlank)
                {
                    IEnumerable<RdfNode> listElements;
                    if (_tripleSource.TryGetListElements(subject, out listElements))
                    {
                        yield return Convert(listElements).ToList();
                    }
                    else
                    {
                        yield return _entityFactory.Create(new BlankId(subject.BlankNodeId, subject.GraphUri));
                    }
                }
                else
                {
                    object value;
                    if (TryConvert(subject, out value))
                    {
                        yield return value;
                    }
                    else
                    {
                        yield return subject.Literal;
                    }
                }
            }
        }

        // todo: refactor this functionality to a specialized class (multiple implementations stored in a lookup dictionary?)
        private bool TryConvert(RdfNode subject, out object value)
        {
            if (subject.DataType != null)
            {
                switch (subject.DataType.ToString())
                {
                    case "http://www.w3.org/2001/XMLSchema#int":
                    case "http://www.w3.org/2001/XMLSchema#integer":
                        int integer;
                        if (int.TryParse(subject.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out integer))
                        {
                            value = integer;
                            return true;
                        }
                        break;
                }
            }

            value = null;
            return false;
        }
    }
}