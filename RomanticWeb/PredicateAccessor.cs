using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NullGuard;

namespace RomanticWeb
{
    internal interface IPredicateAccessor
    {
        dynamic GetObjects(Uri baseUri, Property predicate);
    }

    [NullGuard(ValidationFlags.OutValues)] 
    public abstract class PredicateAccessor<TTriplesSource> : DynamicObject, IPredicateAccessor
    {
        private readonly TTriplesSource _tripleSource;
        private readonly EntityId _entityId;
        private readonly Ontology _ontology;

        protected PredicateAccessor(TTriplesSource tripleSource, Entity entity, Ontology ontology)
        {
            _tripleSource = tripleSource;
            _entityId = entity.Id;
            _ontology = ontology;
        }

        protected EntityId EntityId
        {
            get { return _entityId; }
        }

        dynamic IPredicateAccessor.GetObjects(Uri baseUri, Property predicate)
        {
            var subjectValues = GetObjects(_tripleSource, baseUri, predicate).ToList();

            if (subjectValues.Count == 1)
            {
                return subjectValues.Single();
            }

            if (subjectValues.Count == 0)
            {
                return null;
            }

            return subjectValues;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var predicate = _ontology.Predicates.Single(p => p.PredicateUri == binder.Name);

            result = ((IPredicateAccessor) this).GetObjects(_ontology.BaseUri, predicate);

            return true;
        }

        protected abstract IEnumerable<string> GetObjects(TTriplesSource triplesSource, Uri baseUri, Property predicate);
    }
}