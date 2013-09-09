using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NullGuard;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    [NullGuard(ValidationFlags.OutValues)]
    public abstract class PredicateAccessor<TTriplesSource> : DynamicObject, IPredicateAccessor
    {
        private readonly TTriplesSource _tripleSource;
        private readonly EntityId _entityId;
        private readonly Ontology _ontology;

        protected PredicateAccessor(TTriplesSource tripleSource, Entity entity, Ontology ontology, IEntityFactory entityFactory)
        {
            _tripleSource = tripleSource;
            _entityId = entity.Id;
            _ontology = ontology;
            EntityFactory = entityFactory;
        }

        protected EntityId EntityId
        {
            get { return _entityId; }
        }

        dynamic IPredicateAccessor.GetObjects(Uri baseUri, Property predicate)
        {
            var subjectValues = GetObjectNodes(_tripleSource, baseUri, predicate);
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
                return EntityFactory.Create(new EntityId(subject.Uri));
            }

            return subject;
        }

        protected IEntityFactory EntityFactory { get; private set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var predicate = _ontology.Predicates.SingleOrDefault(p => p.PredicateUri == binder.Name);

            if (predicate == null)
            {
                throw new UnknownPredicateException(_ontology.BaseUri, binder.Name);
            }

            result = ((IPredicateAccessor)this).GetObjects(_ontology.BaseUri, predicate);

            return true;
        }

        /// <summary>
        /// Gets all RDF objects together with language tags and data type information for literals
        /// </summary>
        protected abstract IEnumerable<RdfNode> GetObjectNodes(TTriplesSource triplesSource, Uri baseUri, Property predicate);
    }
}