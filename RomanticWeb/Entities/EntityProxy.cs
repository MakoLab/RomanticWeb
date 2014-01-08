using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Proxy for exposing mapped entity members
    /// </summary>
    [NullGuard(ValidationFlags.OutValues)]
    public class EntityProxy:DynamicObject,IEntity
    {
        #region Fields
        private static readonly IResultProcessingStrategy FallbackProcessing=new SingleOrDefaultProcessing();

        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMappings;
        private readonly INodeConverter _converter;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProxy"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="entityMappings">The entity mappings.</param>
        /// <param name="converter">The converter.</param>
        public EntityProxy(IEntityStore store,Entity entity,IEntityMapping entityMappings,INodeConverter converter)
        {
            _store=store;
            _entity=entity;
            _entityMappings=entityMappings;
            _converter=converter;
            ResultAggregations=new Lazy<IResultProcessingStrategy,IResultProcessingStrategyMetadata>[0];
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result aggregation strategies.
        /// </summary>
        [ImportMany(typeof(IResultProcessingStrategy))]
        public IEnumerable<Lazy<IResultProcessingStrategy,IResultProcessingStrategyMetadata>> ResultAggregations { get; internal set; }

        /// <summary>Gets the entity's identifier</summary>
        public EntityId Id
        {
            get
            {
                return _entity.Id;
            }
        }

        /// <inheritdoc />
        public dynamic this[string member]
        {
            get
            {
                return _entity[member];
            }
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder,out object result)
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

            LogTo.Trace("Reading property {0} from graph {1}",property.Uri,graph);

            IEnumerable<Node> objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri,graph);
            var objectsForPredicate=_converter.ConvertNodes(objects,property);

            var operation=(!property.IsCollection?ProcessingOperation.SingleOrDefault:
                ((typeof(IDictionary).IsAssignableFrom(property.ReturnType))||(typeof(IDictionary<,>).IsAssignableFromSpecificGeneric(property.ReturnType))?ProcessingOperation.Dictionary:
                    ProcessingOperation.Flatten));
            var aggregation=(from agg in ResultAggregations
                             where agg.Metadata.Operation==operation
                             select agg.Value).SingleOrDefault();

            aggregation=aggregation??FallbackProcessing;

            LogTo.Trace("Performing operation {0} on result nodes",operation);
            var aggregatedResult=aggregation.Process(objectsForPredicate);

            var collection=aggregatedResult as ICollection;
            if (collection!=null)
            {
                INotifyCollectionChanged observable;
                Type[] genericArguments=null;
                IDictionary dictionary=aggregatedResult as IDictionary;
                if (dictionary!=null)
                {
                    genericArguments=new Type[] { typeof(object),typeof(object) };
                    if (typeof(IDictionary<,>).IsAssignableFromSpecificGeneric(dictionary.GetType()))
                    {
                        genericArguments=dictionary.GetType().GetGenericArguments().Take(2).ToArray();
                    }

                    observable=(INotifyCollectionChanged)typeof(ObservableDictionary<,>)
                        .MakeGenericType(genericArguments)
                        .GetConstructor(new Type[] { typeof(IDictionary) })
                        .Invoke(new object[] { dictionary });
                }
                else
                {
                    genericArguments=new Type[] { typeof(object) };
                    if (typeof(IEnumerable<>).IsAssignableFromSpecificGeneric(collection.GetType()))
                    {
                        genericArguments=collection.GetType().GetGenericArguments().Take(1).ToArray();
                    }

                    observable=(INotifyCollectionChanged)typeof(ObservableCollection<>)
                        .MakeGenericType(genericArguments)
                        .GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(genericArguments) })
                        .Invoke(new object[] { collection.Cast<object>() });
                }

                observable.CollectionChanged+=(sender,args) => Impromptu.InvokeSet(this,binder.Name,sender);
                result=observable;
            }
            else
            {
                result=aggregatedResult;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool TrySetMember(SetMemberBinder binder,object value)
        {
            _entity.EnsureIsInitialized();

            var property=_entityMappings.PropertyFor(binder.Name);

            LogTo.Trace("Setting property {0}",property.Uri);

            var propertyUri=Node.ForUri(property.Uri);
            var graphUri=property.GraphSelector.SelectGraph(_entity.Id);

            ////if (property.IsCollection)
            ////{
            ////    var newValue = Node.ForLiteral(value.ToString());

            ////    _store.ReplaceCollection(_entity.Id, propertyUri, newValue, graphUri);
            ////}
            ////else
            {
                var newValues=_converter.ConvertBack(value,property);

                _store.ReplacePredicateValues(_entity.Id,propertyUri,newValues,graphUri);
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var entity=obj as IEntity;
            if (entity==null) { return false; }

            return entity.Equals(_entity);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _entity.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _entity.ToString();
        }

        /// <summary>
        /// Gets the underlying wrapper as another type of entity.
        /// </summary>
        public TInterface AsEntity<TInterface>() where TInterface:class,IEntity
        {
            return _entity.AsEntity<TInterface>();
        }

        /// <summary>
        /// Gets the undelying entity as dynamic.
        /// </summary>
        public dynamic AsDynamic()
        {
            return _entity;
        }
        #endregion
    }
}