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
            var subjectValues = GetObjects(_tripleSource, baseUri, predicate);
            var subjects = (from subject in subjectValues
                            select Convert(subject, predicate)).ToList();

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

        private object Convert(string subject, Property predicate)
        {
            if (predicate is ObjectProperty)
            {
                return EntityFactory.Create(new EntityId(subject));
            }

            return subject;
        }

        protected IEntityFactory EntityFactory { get; private set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var predicate = _ontology.Predicates.Single(p => p.PredicateUri == binder.Name);

            result = ((IPredicateAccessor)this).GetObjects(_ontology.BaseUri, predicate);

            return true;
        }

        protected abstract IEnumerable<string> GetObjects(TTriplesSource triplesSource, Uri baseUri, Property predicate);
    }
}