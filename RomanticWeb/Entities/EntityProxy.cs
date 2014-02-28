using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Anotar.NLog;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
    /// <summary>Proxy for exposing mapped entity members.</summary>
    [NullGuard(ValidationFlags.All^ValidationFlags.OutValues)]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerDisplayProxy))]
    public class EntityProxy:DynamicObject,IEntity,IResultProcessingStrategyClient
    {
        #region Fields
        private static readonly IResultProcessingStrategy FallbackProcessing=new SingleOrDefaultProcessing();

        private readonly IEntityStore _store;
        private readonly Entity _entity;
        private readonly IEntityMapping _entityMapping;
        private readonly INodeConverter _converter;

        private NamedGraphSelectionOverride _namedGraphSelectionOverride;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="EntityProxy"/> class.</summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityMapping">The entity mappings.</param>
        public EntityProxy(Entity entity,IEntityMapping entityMapping)
        {
            _store=entity.Context.Store;
            _entity=entity;
            _entityMapping=entityMapping;
            _converter=entity.Context.NodeConverter;
        }
        #endregion

        #region Properties
        /// <summary>Gets the result aggregation strategies.</summary>
        IDictionary<ProcessingOperation,IResultProcessingStrategy> IResultProcessingStrategyClient.ResultAggregations { get { return this.GetResultAggregations(); } }

        /// <inheritdoc/>
        public EntityId Id
        {
            get
            {
                return _entity.Id;
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
            var aggregatedResult=GetValues(property);

            if (property.IsCollection)
            {
                ////INotifyCollectionChanged observable;
                Type[] genericArguments=null;
                ////IDictionary dictionary=aggregatedResult as IDictionary;
                ////if (dictionary!=null)
                ////{
                ////    genericArguments = property.ReturnType.GetGenericArguments();

                ////    observable=(INotifyCollectionChanged)typeof(ObservableDictionary<,>)
                ////        .MakeGenericType(genericArguments)
                ////        .GetConstructor(new Type[] { typeof(IDictionary) })
                ////        .Invoke(new object[] { dictionary });
                ////    observable.CollectionChanged += (sender, args) => Impromptu.InvokeSet(this, binder.Name, sender);
                ////    result = observable;
                ////}
                ////else
                if (property.StorageStrategy==StorageStrategyOption.RdfList)
                {
                    genericArguments=property.ReturnType.GetGenericArguments();

                    if (typeof(IEntity).IsAssignableFrom(genericArguments.Single()))
                    {
                        genericArguments=new[] { typeof(IEntity) };
                    }

                    var ctor=
                        typeof(RdfListAdapter<>).MakeGenericType(genericArguments)
                                                .GetConstructor(new[] { typeof(IEntityContext),typeof(IRdfListNode) });

                    IRdfListNode head=((IEntity)aggregatedResult).AsEntity<IRdfListNode>();
                    EnsureNamedGraphOverrideInChildEntity((EntityProxy)head.UnwrapProxy(),property);

                    result=ctor.Invoke(new object[] { Context,head });
                }
                else
                {
                    genericArguments=property.ReturnType.GetGenericArguments();
                    if (typeof(IEntity).IsAssignableFrom(genericArguments.Single()))
                    {
                        genericArguments=new[] { typeof(IEntity) };
                    }

                    var castMethod=
                        Info.OfMethod("System.Core","System.Linq.Enumerable","Cast","IEnumerable")
                            .MakeGenericMethod(genericArguments);

                    var convertedCollection=castMethod.Invoke(null,new[] { aggregatedResult });
                    var observable=(INotifyCollectionChanged)
                                                        typeof(ObservableCollection<>).MakeGenericType(genericArguments)
                                                                                      .GetConstructor(
                                                                                          new Type[]
                                                                                              {
                                                                                                  typeof(IEnumerable<>).MakeGenericType(genericArguments)
                                                                                              })
                                                                                      .Invoke(new[] { convertedCollection });

                    observable.CollectionChanged+=(sender,args) => Impromptu.InvokeSet(this,binder.Name,sender);
                    result=observable;
                }
            }
            else
            {
                if (aggregatedResult is IRdfListNode)
                {
                    EnsureNamedGraphOverrideInChildEntity((EntityProxy)((IRdfListNode)aggregatedResult).UnwrapProxy(),property);
                }

                result=aggregatedResult;
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
            var graphUri=Context.GraphSelector.SelectGraph(Id,_entityMapping,property);

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

        /// <summary>
        /// Gets the undelying entity as dynamic.
        /// </summary>
        public dynamic AsDynamic()
        {
            return _entity;
        }
        #endregion

        #region Private methods

        internal void OverrideNamedGraphSelection(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping propertyMapping)
        {
            OverrideNamedGraphSelection(new NamedGraphSelectionOverride(entityId,entityMapping,propertyMapping));
        }

        private void OverrideNamedGraphSelection(NamedGraphSelectionOverride @override)
        {
            _namedGraphSelectionOverride=@override;
        }

        private void EnsureNamedGraphOverrideInChildEntity(EntityProxy proxy,IPropertyMapping property)
        {
            if (_namedGraphSelectionOverride!=null)
            {
                proxy.OverrideNamedGraphSelection(_namedGraphSelectionOverride);
            }
            else
            {
                proxy.OverrideNamedGraphSelection(Id,_entityMapping,property);
            }
        }
        
        [return:AllowNull]
        private object GetValues(IPropertyMapping property)
        {
            var graph=SelectNamedGraph(property);

            LogTo.Trace("Reading property {0} from graph {1}", property.Uri, graph);

            IEnumerable<Node> objects=_store.GetObjectsForPredicate(_entity.Id,property.Uri,graph);
            var objectsForPredicate= _converter.ConvertNodes(objects,property);

            var operation = ProcessingOperation.SingleOrDefault;
            if ((property.IsCollection) && (property.StorageStrategy != StorageStrategyOption.RdfList))
            {
                operation = ProcessingOperation.Flatten;
            }

            IResultProcessingStrategyClient resultProcessingStrategyClient = this;
            var aggregation = (resultProcessingStrategyClient.ResultAggregations.ContainsKey(operation)
                                 ? resultProcessingStrategyClient.ResultAggregations[operation]
                                 : FallbackProcessing);

            LogTo.Trace("Performing operation {0} on result nodes", operation);
            var aggregatedResult = aggregation.Process(objectsForPredicate);
            return aggregatedResult;
        }

        private object SetNamedGraphOverride(object result)
        {
            ////var proxy=result as EntityProxy;
            ////if (proxy!=null)
            ////{
            ////    if (proxy.Id is BlankId || proxy._entityMapping.EntityType==typeof(IRdfListNode))
            ////    {
            ////        proxy.OverrideNamedGraphSelection(_namedGraphSelectionOverride);
            ////    }
            ////}

            return result;
        }

        private Uri SelectNamedGraph(IPropertyMapping property)
        {
            var entityId=Id;
            var mapping=_entityMapping;
            var propertyMapping=property;

            if (_namedGraphSelectionOverride!=null)
            {
                entityId=_namedGraphSelectionOverride.EntityId;
                mapping=_namedGraphSelectionOverride.EntityMapping;
                propertyMapping=_namedGraphSelectionOverride.PropertyMapping;
            }

            return Context.GraphSelector.SelectGraph(entityId,mapping,propertyMapping);
        }

        #endregion

        private class NamedGraphSelectionOverride
        {
            public NamedGraphSelectionOverride(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping propertyMapping)
            {
                EntityId = entityId;
                EntityMapping = entityMapping;
                PropertyMapping = propertyMapping;
            }

            public EntityId EntityId { get; private set; }

            public IEntityMapping EntityMapping { get; private set; }

            public IPropertyMapping PropertyMapping { get; private set; }
        }

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