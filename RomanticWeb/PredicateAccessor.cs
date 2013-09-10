using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NullGuard;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>
    /// Base class for accessing triple subjects from entities
    /// </summary>
    [NullGuard(ValidationFlags.OutValues)]
    public abstract class PredicateAccessor : DynamicObject, IPredicateAccessor
    {
        private readonly ITriplesSource _tripleSource;
        private readonly EntityId _entityId;
        private readonly Ontology _ontology;
        private readonly IEntityFactory _entityFactory;

        /// <summary>
        /// Creates a new instance of <see cref="PredicateAccessor"/>
        /// </summary>
        /// <param name="tripleSource">underlying RDF source</param>
        /// <param name="entity">the access Entity</param>
        /// <param name="ontology">Ontolgy used to resolve predicate names</param>
        /// <param name="entityFactory">factory used to produce associated Entities</param>
        protected PredicateAccessor(ITriplesSource tripleSource, Entity entity, Ontology ontology, IEntityFactory entityFactory)
        {
            _tripleSource = tripleSource;
            _entityId = entity.Id;
            _ontology = ontology;
            _entityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the accessed Entity's identifies
        /// </summary>
        protected EntityId EntityId
        {
            get { return _entityId; }
        }

        public IEnumerable<Property> KnownProperties
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
                property = new DatatypeProperty(binder.Name).InOntology(_ontology);
            }

            result = ((IPredicateAccessor)this).GetObjects(property);

            return true;
        }

        /// <summary>
        /// Gets all RDF objects together with language tags and data type information for literals
        /// </summary>
        protected abstract IEnumerable<RdfNode> GetObjectNodes(ITriplesSource triplesSource, Property predicate);

        dynamic IPredicateAccessor.GetObjects(Property predicate)
        {
            var subjectValues = GetObjectNodes(_tripleSource, predicate);
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
                return _entityFactory.Create(new EntityId(subject.Uri));
            }

            return subject.Literal;
        }
    }

    public interface ITriplesSource
    {
        IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate);
    }
}