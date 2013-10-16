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

namespace RomanticWeb.Entities
{
    [NullGuard(ValidationFlags.OutValues)]
    internal class EntityProxy:DynamicObject,IEntity
    {
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
        public IEnumerable<Lazy<IResultAggregationStrategy, IResultAggregationStrategyMetadata>> ResultAggregations { get; private set; } 

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

            var objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri);
            var objectsForPredicate=_converter.ConvertNodes(property.Uri,objects);

            LogTo.Debug("Reading property {0}", property.Uri);

            var operation=property.IsCollection?AggregateOperation.Flatten:AggregateOperation.SingleOrDefault;
            var aggregation=(from agg in ResultAggregations 
                             where agg.Metadata.Operation==operation 
                             select agg).SingleOrDefault();

            LogTo.Debug("Performing operation {0} on result nodes", operation);
            result=aggregation.Value.Aggregate(objectsForPredicate);

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