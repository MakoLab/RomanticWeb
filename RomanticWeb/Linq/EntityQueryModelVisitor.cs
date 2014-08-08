using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NullGuard;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Model.Navigators;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Linq
{
    /// <summary>Converts LINQ query model to SPARQL abstraction.</summary>
    //// TODO: Last changes are asking to refactorize LINQ -> SPARQL process, i.e. 
    //// currently we're using UnspecifiedEntityAccessor for properties, while we should stick to the default graph selection policy.
    //// The UnspecifiedEntityAccessor might disappear some day as it's current implementation makes the graph matching condition removed.
    //// Also the filters are added to the accessor on a basis that we feel is somehow inconsistant.
    internal class EntityQueryModelVisitor : QueryModelVisitorBase, IQueryVisitor
    {
        #region Fields
        private readonly IEntityContext _entityContext;
        private EntityQueryVisitor _visitor;
        private Query _query;
        private StrongEntityAccessor _mainFromComponent;
        private IExpression _auxFromComponent;
        private QueryComponent _result;
        private Identifier _subject;
        private IPropertyMapping _propertyMapping;
        #endregion

        #region Constructors
        /// <summary>Default constructor with mappings repository passed.</summary>
        /// <param name="entityContext">Entity context.</param>
        public EntityQueryModelVisitor(IEntityContext entityContext)
            : this(new Query(), entityContext)
        {
        }

        internal EntityQueryModelVisitor(Query query, IEntityContext context)
        {
            _entityContext = context;
            _visitor = new EntityQueryVisitor(_query = (Query)(_result = query), _entityContext);
            _subject = null;
        }
        #endregion

        #region Properties
        /// <summary>Gets a SPARQL abstraction model.</summary>
        public Query Query { get { return _query; } }

        /// <summary>Gets the mappings repository.</summary>
        IMappingsRepository IQueryVisitor.MappingsRepository { get { return _entityContext.Mappings; } }

        /// <summary>Gets the base Uri selection policy.</summary>
        IBaseUriSelectionPolicy IQueryVisitor.BaseUriSelector { get { return _entityContext.BaseUriSelector; } }

        /// <summary>Gets a resulting query.</summary>
        internal QueryComponent Result { get { return _result; } }

        /// <summary>Gets the value converter for property selectors.</summary>
        internal IPropertyMapping PropertyMapping { get { return _propertyMapping; } }
        #endregion

        #region Public methods
        /// <summary>Visits a query model.</summary>
        /// <param name="queryModel">Query model to be visited.</param>
        public override void VisitQueryModel(Remotion.Linq.QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this, queryModel);
        }

        /// <summary>Visits a select clause.</summary>
        /// <param name="selectClause">Select clause to be visited.</param>
        /// <param name="queryModel">Query model containing given select clause.</param>
        public override void VisitSelectClause(SelectClause selectClause, Remotion.Linq.QueryModel queryModel)
        {
            queryModel.MainFromClause.Accept(this, queryModel);
            QuerySourceReferenceExpression querySource = FindQuerySource(selectClause.Selector);
            _visitor.VisitExpression(selectClause.Selector);
            QueryComponent component = _visitor.RetrieveComponent();
            _query = _visitor.Query;
            _mainFromComponent = _query.FindAllComponents<StrongEntityAccessor>().Where(item => item.SourceExpression == querySource.ReferencedQuerySource).First();
            if (_query.Subject == null)
            {
                _query.Subject = _mainFromComponent.About;
                UnboundConstrain genericConstrain = new UnboundConstrain(new Identifier("s"), new Identifier("p"), new Identifier("o"), _mainFromComponent.SourceExpression.FromExpression);
                _mainFromComponent.Elements.Insert(0, genericConstrain);
                _mainFromComponent.UnboundGraphName = null;
                _query.Select.Add(genericConstrain);
            }

            _subject = _query.Subject;
            _query.Select.Add(_mainFromComponent);
            VisitBodyClauses(queryModel.BodyClauses, queryModel);
            VisitResultOperators(queryModel.ResultOperators, queryModel);
            OverrideSelector(component, selectClause.Selector);
        }

        /// <summary>Visits a where clause.</summary>
        /// <param name="whereClause">Where clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitWhereClause(WhereClause whereClause, Remotion.Linq.QueryModel queryModel, int index)
        {
            _visitor.ConstantFromClause = _auxFromComponent;
            _visitor.VisitExpression(whereClause.Predicate);
            QueryComponent queryComponent = _visitor.RetrieveComponent();
            IQueryComponentNavigator queryComponentNavigator = queryComponent.GetQueryComponentNavigator();
            if (queryComponentNavigator != null)
            {
                queryComponentNavigator.ReplaceComponent(Identifier.Current, _subject);
            }

            if (queryComponent is QueryElement)
            {
                if ((!(queryComponent is EntityConstrain)) && (!_query.Elements.Contains((QueryElement)queryComponent)))
                {
                    _query.Elements.Add((QueryElement)queryComponent);
                }
            }
            else if (!_query.FindAllComponents<Filter>().Any(item => item.Expression == queryComponent))
            {
                Filter filter = new Filter((IExpression)queryComponent);
                IEnumerable<StrongEntityAccessor> targetEntityAccessorExression = _query.Elements.OfType<StrongEntityAccessor>();
                if ((_query.IsSubQuery) || (whereClause.Predicate is Remotion.Linq.Clauses.Expressions.SubQueryExpression))
                {
                    targetEntityAccessorExression = targetEntityAccessorExression.Except(new StrongEntityAccessor[] { _query.Elements.OfType<StrongEntityAccessor>().LastOrDefault() });
                }

                StrongEntityAccessor targetEntityAccessor = targetEntityAccessorExression.LastOrDefault() ?? _mainFromComponent;
                if ((!targetEntityAccessor.Elements.Contains(queryComponent)) && (!targetEntityAccessor.Elements.Contains(filter)))
                {
                    targetEntityAccessor.Elements.Add(filter);
                }
            }

            _auxFromComponent = null;
            base.VisitWhereClause(whereClause, queryModel, index);
        }

        /// <summary>Visits an additional from clause.</summary>
        /// <param name="fromClause">From clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            VisitQuerableFromClause(fromClause, queryModel, index);
            StrongEntityAccessor entityAccessor = (fromClause.FromExpression is System.Linq.Expressions.ConstantExpression ? null : _visitor.GetEntityAccessor(fromClause));
            if (entityAccessor != null)
            {
                _query.AddEntityAccessor(entityAccessor);
            }
            else
            {
                NonEntityQueryModelVisitor modelVisitor = new NonEntityQueryModelVisitor(_visitor);
                modelVisitor.VisitAdditionalFromClause(fromClause, queryModel, index);
                if (modelVisitor.From is Identifier)
                {
                    _subject = (Identifier)modelVisitor.From;
                }
                else if (modelVisitor.From is IExpression)
                {
                    _auxFromComponent = (IExpression)modelVisitor.From;
                }
            }

            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        /// <summary>Visits a main from clause.</summary>
        /// <param name="fromClause">Main from clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        public override void VisitMainFromClause(MainFromClause fromClause, Remotion.Linq.QueryModel queryModel)
        {
            VisitQuerableFromClause(fromClause, queryModel, -1);
            if (fromClause.FromExpression is System.Linq.Expressions.MemberExpression)
            {
                System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)fromClause.FromExpression;
                if (memberExpression.Member is PropertyInfo)
                {
                    _visitor.VisitExpression(memberExpression.Expression);
                    _visitor.VisitExpression(memberExpression);
                }
            }

            base.VisitMainFromClause(fromClause, queryModel);
        }

        /// <summary>Visits a result operator.</summary>
        /// <param name="resultOperator">Result operator to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the visited result operator in the result operators collection.</param>
        public override void VisitResultOperator(ResultOperatorBase resultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            MethodInfo visitResultOperatorMethod = GetType().GetMethod("Visit" + resultOperator.GetType().Name, BindingFlags.Instance | BindingFlags.NonPublic);
            if (visitResultOperatorMethod != null)
            {
                visitResultOperatorMethod.Invoke(this, new object[] { resultOperator, queryModel, index });
            }
            else
            {
                throw new NotSupportedException(System.String.Format("Expressions of type '{0}' are not supported.", resultOperator.GetType().Name.Replace("ResultOperator", System.String.Empty)));
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }
        #endregion

        #region Non-public methods
        /// <summary>Visits body clauses.</summary>
        /// <param name="bodyClauses">Body clause to be visited.</param>
        /// <param name="queryModel">Query model containing given body clause.</param>
        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, Remotion.Linq.QueryModel queryModel)
        {
            foreach (var indexValuePair in bodyClauses.AsChangeResistantEnumerableWithIndex())
            {
                indexValuePair.Value.Accept(this, queryModel, indexValuePair.Index);
            }
        }

        /// <summary>Visits order by clauses.</summary>
        /// <param name="orderings">Order by clauses to be visited.</param>
        /// <param name="queryModel">Query model containing given body clause.</param>
        /// <param name="orderByClause">First order by clause.</param>
        protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
        {
            foreach (Ordering ordering in orderings)
            {
                _visitor.VisitExpression(ordering.Expression);
                QueryComponent component = _visitor.RetrieveComponent();
                if (component is IExpression)
                {
                    IExpression expression = (IExpression)component;
                    if (!_query.OrderBy.ContainsKey(expression))
                    {
                        _query.OrderBy[expression] = (ordering.OrderingDirection == OrderingDirection.Desc);
                    }
                }
            }
        }

        /// <summary>Visits a from clause.</summary>
        /// <param name="fromClause">From clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model. In case of the main from clause this value is -1.</param>
        protected virtual void VisitQuerableFromClause(FromClauseBase fromClause, Remotion.Linq.QueryModel queryModel, int index)
        {
            if (typeof(IQueryable).IsAssignableFrom(fromClause.FromExpression.Type))
            {
                if ((fromClause.FromExpression.Type.GetGenericArguments().Length > 0) && (fromClause.FromExpression.Type.GetGenericArguments()[0] != typeof(IEntity)))
                {
                    StrongEntityAccessor entityAccessor = this.GetEntityAccessor(fromClause);
                    if (_mainFromComponent == null)
                    {
                        _query.Elements.Add(_mainFromComponent = entityAccessor);
                    }
                }
            }
            else
            {
                _visitor.VisitExpression(fromClause.FromExpression);
            }
        }

        private void VisitAnyResultOperator(AnyResultOperator anyResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            if (_query.IsSubQuery)
            {
                Call call = new Call(MethodNames.Any);
                call.Arguments.Add(_query);
                _result = call;
            }
            else
            {
                _query.QueryForm = QueryForms.Ask;
            }
        }

        private void VisitContainsResultOperator(ContainsResultOperator containsResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            if (_query.IsSubQuery)
            {
                _visitor.VisitExpression(containsResultOperator.Item);
                QueryComponent item = _visitor.RetrieveComponent();
                if (item is IExpression)
                {
                    Filter filter = new Filter(new BinaryOperator(MethodNames.Equal, _mainFromComponent.About, (IExpression)item));
                    if (!_mainFromComponent.Elements.Contains(filter))
                    {
                        _mainFromComponent.Elements.Add(filter);
                    }
                }

                EntityConstrain constrain = new EntityConstrain(
                    new Identifier(_mainFromComponent.About.Name + "_p"),
                    new Identifier(_mainFromComponent.About.Name + "_o"),
                    containsResultOperator.Item);
                if (!_mainFromComponent.Elements.Contains(constrain))
                {
                    _mainFromComponent.Elements.Add(constrain);
                }

                Call call = new Call(MethodNames.Any);
                call.Arguments.Add(_query);
                _result = call;
            }
            else
            {
                throw new NotSupportedException(System.String.Format("Cannot perform 'Contains' operation on top level query."));
            }
        }

        private void VisitCountResultOperator(CountResultOperator countResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            Call distinct = new Call(MethodNames.Distinct);
            distinct.Arguments.Add(_mainFromComponent.About);
            UnboundConstrain constrain = (UnboundConstrain)_mainFromComponent.Elements.FirstOrDefault(item => item.GetType() == typeof(UnboundConstrain));
            if (constrain != null)
            {
                _mainFromComponent.Elements.Remove(constrain);
            }

            Call count = new Call(MethodNames.Count);
            count.Arguments.Add(distinct);
            _query.Select.Clear();
            Alias alias = new Alias(count, new Identifier(_query.CreateVariableName(_query.RetrieveIdentifier(_mainFromComponent.About.Name) + "Count"), typeof(int)));
            _query.Select.Add(alias);
        }

        private void VisitFirstResultOperator(FirstResultOperator countResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
        }

        private void VisitSingleResultOperator(SingleResultOperator countResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
        }

        private void VisitSkipResultOperator(SkipResultOperator skipResultOperation, Remotion.Linq.QueryModel queryModel, int index)
        {
            if (!(skipResultOperation.Count is System.Linq.Expressions.ConstantExpression))
            {
                throw new InvalidOperationException("Only constant expressions are supported for the Skip operators.");
            }

            _query.Offset = Convert.ToInt32(((System.Linq.Expressions.ConstantExpression)skipResultOperation.Count).Value);
        }

        private void VisitTakeResultOperator(TakeResultOperator takeResultOperation, Remotion.Linq.QueryModel queryModel, int index)
        {
            if (!(takeResultOperation.Count is System.Linq.Expressions.ConstantExpression))
            {
                throw new InvalidOperationException("Only constant expressions are supported for the Take operators.");
            }

            _query.Limit = Convert.ToInt32(((System.Linq.Expressions.ConstantExpression)takeResultOperation.Count).Value);
        }

        private void OverrideSelector(QueryComponent component, System.Linq.Expressions.Expression selector)
        {
            if ((component is Identifier) && (selector is System.Linq.Expressions.MemberExpression))
            {
                System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)selector;
                if (!(memberExpression.Member is PropertyInfo))
                {
                    throw new NotSupportedException(System.String.Format("Selection on members of type '{0}' are not supported.", memberExpression.Member.MemberType));
                }

                PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.DeclaringType))
                {
                    _propertyMapping = _entityContext.Mappings.FindPropertyMapping((PropertyInfo)memberExpression.Member);
                    if (typeof(IEntity).IsAssignableFrom(_propertyMapping.ReturnType))
                    {
                        OverrideEntitySelector((Identifier)component);
                    }
                    else
                    {
                        OverrideLiteralSelector(this.GetEntityAccessor((FromClauseBase)FindQuerySource(memberExpression).ReferencedQuerySource));
                    }
                }
            }
            else if (component is StrongEntityAccessor)
            {
                _propertyMapping = IdentifierPropertyMapping.Default;
                OverrideIdentifierSelector((StrongEntityAccessor)component);
            }
        }

        private void OverrideEntitySelector(Identifier identifier)
        {
            StrongEntityAccessor entityAccessor = _query.FindAllComponents<StrongEntityAccessor>().Where(item => item.About == identifier).FirstOrDefault();
            if (entityAccessor == null)
            {
                entityAccessor = new StrongEntityAccessor(identifier);
                _query.Elements.Add(entityAccessor);
            }

            if ((entityAccessor.UnboundGraphName == null) || (entityAccessor.UnboundGraphName == entityAccessor.About))
            {
                entityAccessor.UnboundGraphName = (from accessor in _query.FindAllComponents<StrongEntityAccessor>()
                                                   from constrain in accessor.Elements.OfType<EntityConstrain>()
                                                   where (constrain.Value is Identifier) && ((Identifier)constrain.Value == identifier)
                                                   select accessor.About).FirstOrDefault() ?? identifier;
                UnboundConstrain genericConstrain = new UnboundConstrain();
                genericConstrain.Subject = identifier;
                genericConstrain.Predicate = new Identifier(identifier.Name + "P");
                genericConstrain.Value = new Identifier(identifier.Name + "O");
                entityAccessor.Elements.Add(genericConstrain);
                _query.Select.Clear();
                _query.Select.Add(genericConstrain);
                _query.Select.Add(entityAccessor);

                if (_mainFromComponent.Elements[0] is UnboundConstrain)
                {
                    _mainFromComponent.Elements.RemoveAt(0);
                }
            }
        }

        private void OverrideLiteralSelector(StrongEntityAccessor entityAccessor)
        {
            if (_mainFromComponent.Elements[0] is UnboundConstrain)
            {
                entityAccessor.Elements.Add(new Filter(new BinaryOperator(MethodNames.Equal, ((UnboundConstrain)_mainFromComponent.Elements[0]).Predicate, new Literal(_propertyMapping.Uri))));
            }
        }

        private void OverrideIdentifierSelector(StrongEntityAccessor entityAccessor)
        {
            IdentifierEntityAccessor identifierEntityAccessor = new IdentifierEntityAccessor(entityAccessor.About, entityAccessor);
            int indexOf = -1;
            if ((indexOf = _query.Elements.IndexOf(entityAccessor)) != -1)
            {
                _query.Elements.RemoveAt(indexOf);
                _query.Elements.Insert(indexOf, identifierEntityAccessor);
            }

            _query.Select.Clear();
            _query.Select.Add(identifierEntityAccessor);
            _mainFromComponent = identifierEntityAccessor;
        }

        private QuerySourceReferenceExpression FindQuerySource(System.Linq.Expressions.Expression expression)
        {
            QuerySourceReferenceExpression result = null;
            if (expression is QuerySourceReferenceExpression)
            {
                result = (QuerySourceReferenceExpression)expression;
            }
            else if (expression is System.Linq.Expressions.MemberExpression)
            {
                result = FindQuerySource(((System.Linq.Expressions.MemberExpression)expression).Expression);
            }

            return result;
        }
        #endregion

        internal sealed class IdentifierPropertyMapping : IPropertyMapping
        {
            /// <summary>Gets the default instance of the <see cref="IdentifierPropertyMapping" />.</summary>
            public static readonly IdentifierPropertyMapping Default = new IdentifierPropertyMapping();

            private IdentifierPropertyMapping()
            {
            }

            /// <inheritdoc />
            IEntityMapping IPropertyMapping.EntityMapping { get { return null; } }

            /// <inheritdoc />
            Uri IPropertyMapping.Uri { get { return Rdf.subject; } }

            /// <inheritdoc />
            public string Name { get { return "Id"; } }

            /// <inheritdoc />
            public Type ReturnType { get { return typeof(EntityId); } }

            /// <inheritdoc />
            public Type DeclaringType { get { return typeof(IEntity); } }

            /// <inheritdoc />
            INodeConverter IPropertyMapping.Converter { get { return null; } }

            /// <inheritdoc />
            void IPropertyMapping.Accept(Mapping.Visitors.IMappingModelVisitor mappingModelVisitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}