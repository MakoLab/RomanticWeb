using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    internal class EntityProxy<TEntity> : DynamicObject
    {
        private readonly ITriplesSource _source;
        private readonly EntityId _entityId;
        private readonly IMapping<TEntity> _mappings;
        private readonly RdfNodeConverter _converter;

        public EntityProxy(ITriplesSource source, EntityId entity, IMapping<TEntity> mappings, RdfNodeConverter converter)
        {
            _source = source;
            _entityId = entity;
            _mappings = mappings;
            _converter = converter;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Property property = _mappings.PropertyFor(binder.Name);

            IList objectsForPredicate = _converter.Convert(_source.GetObjectsForPredicate(_entityId, property), _source).ToList();

            if (objectsForPredicate.Count == 1)
            {
                result = objectsForPredicate[0];
            }
            else if (objectsForPredicate.Count == 0)
            {
                result = null;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Multiple results for predicate {0}", property));
            }

            return true;
        }
    }
}