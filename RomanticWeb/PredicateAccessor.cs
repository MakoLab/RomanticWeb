using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetSubjects(Uri baseUri, Property predicate);
    }

    public abstract class PredicateAccessor<TTriplesSource> : DynamicObject, IPredicateAccessor
    {
        private readonly TTriplesSource _tripleSource;
        private readonly EntityId _entityId;
        private readonly Ontology _ontology;

        protected PredicateAccessor(TTriplesSource tripleSource, EntityId entityId, Ontology ontology)
        {
            _tripleSource = tripleSource;
            _entityId = entityId;
            _ontology = ontology;
        }

        protected EntityId EntityId
        {
            get { return _entityId; }
        }

        dynamic IPredicateAccessor.GetSubjects(Uri baseUri, Property predicate)
        {
            var subjectValues = GetSubjects(_tripleSource, baseUri, predicate).ToList();

            return subjectValues;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var predicate = _ontology.Predicates.Single(p => p.PredicateUri == binder.Name);

            result = ((IPredicateAccessor) this).GetSubjects(_ontology.BaseUri, predicate);

            return true;
        }

        protected abstract IEnumerable<string> GetSubjects(TTriplesSource triplesSource, Uri baseUri, Property predicate);
    }
}