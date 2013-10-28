using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
    [NullGuard(ValidationFlags.OutValues)]
    internal class EntityProxy:DynamicObject,IEntity
    {
        private static readonly IResultAggregationStrategy FallbackAggregation = new SingleOrDefaultAggregation();

        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMappings;
        private readonly INodeConverter _converter;

        public EntityProxy(IEntityStore store, Entity entity, IEntityMapping entityMappings, INodeConverter converter)
        {
            _store = store;
            _entity = entity;
            _entityMappings = entityMappings;
            _converter = converter;
            ResultAggregations = new Lazy<IResultAggregationStrategy, IResultAggregationStrategyMetadata>[0];
        }

        [ImportMany(typeof(IResultAggregationStrategy))]
        public IEnumerable<Lazy<IResultAggregationStrategy, IResultAggregationStrategyMetadata>> ResultAggregations { get; internal set; } 

        public EntityId Id
        {
            get
            {
                return _entity.Id;
            }
        }

        public dynamic this[string member]
        {
            get
            {
                return _entity[member];
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _entity.EnsureIsInitialized();

            var property=_entityMappings.PropertyFor(binder.Name);

            LogTo.Debug("Reading property {0}", property.Uri);

            var objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri);
            var objectsForPredicate=_converter.ConvertNodes(property.Uri,objects);

            var operation=property.IsCollection?AggregateOperation.Flatten:AggregateOperation.SingleOrDefault;
            var aggregation=(from agg in ResultAggregations 
                             where agg.Metadata.Operation==operation 
                             select agg.Value).SingleOrDefault();

            aggregation=aggregation??FallbackAggregation;

            LogTo.Debug("Performing operation {0} on result nodes",operation);
            result=aggregation.Aggregate(objectsForPredicate);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _entity.EnsureIsInitialized();

            var property = _entityMappings.PropertyFor(binder.Name);

            LogTo.Debug("Setting property {0}", property.Uri);

            var entityId=Node.ForUri(_entity.Id.Uri);
            var propertyUri=Node.ForUri(property.Uri);
            var quadsRemoved=from quad in _store.Quads 
                             where quad.EntityId==_entity.Id
                             && quad.Predicate==propertyUri
                             && quad.Subject==entityId
                             select quad;

            var graphUri=property.GraphSelector.SelectGraph(_entity.Id);
            if (graphUri!=null)
            {
                quadsRemoved=quadsRemoved.Where(quad => quad.Graph==Node.ForUri(graphUri));
            }

            foreach (var entityTriple in quadsRemoved.ToList())
            {
                _store.RetractTriple(entityTriple);
            }

            var newValue=Node.ForLiteral(value.ToString());
            _store.AssertTriple(new EntityTriple(_entity.Id, entityId, propertyUri, newValue).InGraph(graphUri));

            return true;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as IEntity;
            if (entity == null) { return false; }

            return entity.Equals(_entity);
        }

        public override int GetHashCode()
        {
            return _entity.GetHashCode();
        }

        public override string ToString()
        {
            return _entity.ToString();
        }

        public TInterface AsEntity<TInterface>() where TInterface : class,IEntity
        {
            return _entity.AsEntity<TInterface>();
        }

        public dynamic AsDynamic()
        {
            return _entity;
        }
    }
}