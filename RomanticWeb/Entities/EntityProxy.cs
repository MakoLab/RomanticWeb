using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using Anotar.NLog;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    /// <summary>Proxy for exposing mapped entity members.</summary>
    [NullGuard(ValidationFlags.All^ValidationFlags.OutValues)]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerDisplayProxy))]
    public class EntityProxy:DynamicObject,IEntity,IEntityProxy
    {
        #region Fields
        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMapping;
        private readonly IResultTransformerCatalog _resultTransformers;
        private readonly INodeConverter _converter;
        private ISourceGraphSelectionOverride _overrideSourceGraph;

        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="EntityProxy"/> class.</summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityMapping">The entity mappings.</param>
        public EntityProxy(Entity entity, IEntityMapping entityMapping,IResultTransformerCatalog resultTransformers)
        {
            _store=entity.Context.Store;
            _entity=entity;
            _entityMapping=entityMapping;
            _resultTransformers=resultTransformers;
            _converter=entity.Context.NodeConverter;
        }
        #endregion

        #region Properties

        /// <inheritdoc/>
        public EntityId Id
        {
            get
            {
                return _entity.Id;
            }
        }

        public IEntityMapping EntityMapping
        {
            get
            {
                return _entityMapping;
            }
        }

        /// <inheritdoc/>
        public IEntityContext Context
        {
            get
            {
                return _entity.Context;
            }
        }

        public ISourceGraphSelectionOverride GraphSelectionOverride
        {
            [return:AllowNull]
            get
            {
                return _overrideSourceGraph;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return _entity.ToString();
            }
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder,out object result)
        {
            _entity.EnsureIsInitialized();

            var property=_entityMapping.PropertyFor(binder.Name);
            var graph=SelectNamedGraph(property);

            LogTo.Trace("Reading property {0} from graph {1}",property.Uri,graph);

            IEnumerable<Node> objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri,graph);
            var objectsForPredicate=_converter.ConvertNodes(objects,property);
            var aggregatedResult=AggregateResults(property,objectsForPredicate);
            var resultTransformer=_resultTransformers.GetTransformer(property);

            result=resultTransformer.GetTransformed(this,property,Context,aggregatedResult);

            if (result is IEntity)
            {
                var entityProxy = ((IEntity)result).UnwrapProxy() as IEntityProxy;

                if (entityProxy != null)
                {
                    SetNamedGraphOverride(entityProxy,property);
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override bool TrySetMember(SetMemberBinder binder,object value)
        {
            _entity.EnsureIsInitialized();

            var property=_entityMapping.PropertyFor(binder.Name);

            LogTo.Trace("Setting property {0}",property.Uri);

            var propertyUri=Node.ForUri(property.Uri);
            var graphUri=SelectNamedGraph(property);

            if (property is ICollectionMapping && ((ICollectionMapping)property).StorageStrategy == StorageStrategyOption.RdfList)
            {
                if (value is IRdfListAdapter)
                {
                    Node listHead=Node.FromEntityId((value as IRdfListAdapter).Head.Id);
                    _store.ReplacePredicateValues(Id,propertyUri,new[] { listHead },graphUri);
                }
                else
                {
                    var genericArguments = property.ReturnType.GetGenericArguments();
                    var ctor =
                        typeof(RdfListAdapter<>).MakeGenericType(genericArguments)
                                                .GetConstructor(new[] { typeof(IEntityContext), typeof(OverridingGraphSelector) });
                    var paremeters = GraphSelectionOverride ?? new OverridingGraphSelector(Id, _entityMapping, property);
                    var rdfList=(IRdfListAdapter)ctor.Invoke(new object[] { Context,paremeters });

                    foreach (var item in (IEnumerable)value)
                    {
                        rdfList.Add(item);
                    }

                    Node listHead=Node.FromEntityId(rdfList.Head.Id);
                    _store.ReplacePredicateValues(Id, propertyUri, new[] { listHead },graphUri);
                }
            }
            else
            {
                var newValues=_converter.ConvertBack(value,property);
                _store.ReplacePredicateValues(Id,propertyUri,newValues,graphUri);
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var entity=obj as IEntity;
            if (entity==null) { return false; }

            return _entity.Equals(entity);
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

        /// <summary>Gets the underlying wrapper as another type of entity.</summary>
        public dynamic AsEntity(Type TInterface)
        {
            if (!(typeof(IEntity).IsAssignableFrom(TInterface)))
            {
                throw new ArgumentNullException("TInterface");
            }

            return _entity.AsEntity(TInterface);
        }

        /// <summary>
        /// Gets the underlying entity as dynamic.
        /// </summary>
        public dynamic AsDynamic()
        {
            return _entity;
        }

        public void OverrideGraphSelection(ISourceGraphSelectionOverride parametersOverride)
        {
            _overrideSourceGraph = parametersOverride;
        }
        #endregion

        #region Private methods

        [return:AllowNull]
        private object AggregateResults(IPropertyMapping property,IEnumerable<object> objectsForPredicate)
        {
            try
            {
                var aggregator=_resultTransformers.GetAggregator(property.Aggregation.GetValueOrDefault());
                return aggregator.Aggregate(objectsForPredicate);
            }
            catch (CardinalityException e)
            {
                LogTo.Fatal(
                    "Expected {0} but got {1} result(s) for property {2}",
                    e.ExpectedCardinality,
                    e.ActualCardinality,
                    property);
                throw;
            }
        }

        private void SetNamedGraphOverride(IEntityProxy proxy, IPropertyMapping property)
        {
            var paremeters = GraphSelectionOverride ?? new OverridingGraphSelector(Id, _entityMapping, property);
            if (proxy != null)
            {
                if (proxy.Id is BlankId || proxy.EntityMapping.EntityType == typeof(IRdfListNode))
                {
                    proxy.OverrideGraphSelection(paremeters);
                }
            }
        }

        [return:AllowNull]
        private Uri SelectNamedGraph(IPropertyMapping property)
        {
            if (_overrideSourceGraph!=null)
            {
                return _overrideSourceGraph.SelectGraph(Context.GraphSelector);
            }

            return Context.GraphSelector.SelectGraph(Id,_entityMapping,property);
        }

        private object WrapResultAsDictionary(GetMemberBinder binder, IPropertyMapping property, IDictionary dictionary)
        {
            object result;
            var genericArguments = property.ReturnType.GetGenericArguments();

            var observable =
                (INotifyCollectionChanged)
                typeof(ObservableDictionary<,>).MakeGenericType(genericArguments)
                                               .GetConstructor(new Type[] { typeof(IDictionary) })
                                               .Invoke(new object[] { dictionary });
            observable.CollectionChanged += (sender, args) => Impromptu.InvokeSet(this, binder.Name, sender);
            result = observable;
            return result;
        }

        #endregion

        private class DebuggerDisplayProxy
        {
            private readonly EntityProxy _proxy;

            public DebuggerDisplayProxy(EntityProxy proxy)
            {
                _proxy = proxy;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Entity Entity
            {
                get
                {
                    return _proxy._entity;
                }
            }
        }
    }
}