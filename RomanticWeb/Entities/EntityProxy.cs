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
        private static readonly IResultProcessingStrategy FallbackProcessing = new SingleOrDefaultProcessing();

        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMappings;
        private readonly INodeConverter _converter;

        public EntityProxy(IEntityStore store, Entity entity, IEntityMapping entityMappings, INodeConverter converter)
        {
            if (store == null) { throw new ArgumentNullException("store"); }
            if (entity == null) { throw new ArgumentNullException("entity"); }
            if (converter == null) { throw new ArgumentNullException("converter"); }

            _store = store;
            _entity = entity;
            _entityMappings = entityMappings;
            _converter = converter;
            ResultAggregations = new Lazy<IResultProcessingStrategy, IResultProcessingStrategyMetadata>[0];
        }

        [ImportMany(typeof(IResultProcessingStrategy))]
        public IEnumerable<Lazy<IResultProcessingStrategy, IResultProcessingStrategyMetadata>> ResultAggregations { get; internal set; } 

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
            if (_entityMappings==null)
            {
                // no mapping probably means that this is IEntity
                result=null;
                return false;
            }

            _entity.EnsureIsInitialized();

            var property=_entityMappings.PropertyFor(binder.Name);
            var graph=property.GraphSelector.SelectGraph(_entity.Id);

            LogTo.Debug("Reading property {0} from graph {1}",property.Uri,graph);

            var objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri,graph);
            var objectsForPredicate=_converter.ConvertNodes(objects,property);

            var operation=property.IsCollection?ProcessingOperation.Flatten:ProcessingOperation.SingleOrDefault;
            var aggregation=(from agg in ResultAggregations 
                             where agg.Metadata.Operation==operation 
                             select agg.Value).SingleOrDefault();

            aggregation=aggregation??FallbackProcessing;

            LogTo.Debug("Performing operation {0} on result nodes",operation);
            result=aggregation.Process(objectsForPredicate);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _entity.EnsureIsInitialized();

            var property = _entityMappings.PropertyFor(binder.Name);

            LogTo.Trace("Setting property {0}", property.Uri);

            var propertyUri = Node.ForUri(property.Uri);
            var graphUri = property.GraphSelector.SelectGraph(_entity.Id);

            ////if (property.IsCollection)
            ////{
            ////    var newValue = Node.ForLiteral(value.ToString());

            ////    _store.ReplaceCollection(_entity.Id, propertyUri, newValue, graphUri);
            ////}
            ////else
            {
                var newValues=_converter.ConvertBack(value,property);

                _store.ReplacePredicateValues(_entity.Id, propertyUri, newValues, graphUri);
            }

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