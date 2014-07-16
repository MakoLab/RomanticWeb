using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using RomanticWeb.Linq.Expressions;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq
{
    /// <summary>Converts LINQ query model to SPARQL abstraction, but for LINQ parts that should not create sandalone SPARQL queries.</summary>
    internal class NonEntityQueryModelVisitor : QueryModelVisitorBase
    {
        #region Fields
        private EntityQueryVisitor _visitor;
        private QueryComponent _result;
        private QueryComponent _from;
        private IExpression _mainFromComponent;
        private IList<QueryComponent> _bodies;
        private System.Linq.Expressions.MemberExpression _fromExpression;
        #endregion

        #region Constructors
        internal NonEntityQueryModelVisitor(EntityQueryVisitor queryVisitor, IExpression mainFromComponent = null)
        {
            _visitor = queryVisitor;
            _from = null;
            _mainFromComponent = mainFromComponent;
            _bodies = new List<QueryComponent>();
        }
        #endregion

        #region Properties
        /// <summary>Gets a SPARQL abstraction model.</summary>
        public Query Query { get { return _visitor.Query; } }

        /// <summary>Gets a resulting query.</summary>
        internal QueryComponent Result { get { return _result; } }

        /// <summary>Gets a from component.</summary>
        internal QueryComponent From { get { return _from; } }
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
            if (queryModel.ResultOperators.Count == 0)
            {
                throw new InvalidOperationException("Must have an evaluating expression for sub-queries, i.e. 'Count' or 'Contains'.");
            }

            string currentItemNameOverride = _visitor.ItemNameOverride;
            QuerySourceReferenceExpression selector = (QuerySourceReferenceExpression)selectClause.Selector;
            if (selector.ReferencedQuerySource is MainFromClause)
            {
                MainFromClause mainFrom = (MainFromClause)selector.ReferencedQuerySource;
                if (mainFrom.FromExpression is System.Linq.Expressions.MemberExpression)
                {
                    _fromExpression = (System.Linq.Expressions.MemberExpression)mainFrom.FromExpression;
                }
            }

            _visitor.ItemNameOverride = ((QuerySourceReferenceExpression)selectClause.Selector).ReferencedQuerySource.ItemName;
            queryModel.MainFromClause.Accept(this, queryModel);
            VisitBodyClauses(queryModel.BodyClauses, queryModel);
            VisitResultOperators(queryModel.ResultOperators, queryModel);

            IQueryComponentNavigator resultNavigator = _result.GetQueryComponentNavigator();
            if ((_from != null) && (_from is IExpression))
            {
                if (_mainFromComponent == null)
                {
                    _mainFromComponent = (IExpression)_from;
                }
                else
                {
                    resultNavigator.ReplaceComponent(Identifier.Current, _from);
                }
            }

            if (_mainFromComponent != null)
            {
                if (_bodies.Count == 0)
                {
                    resultNavigator.AddComponent(_mainFromComponent);
                }
                else
                {
                    foreach (QueryComponent queryComponent in _bodies)
                    {
                        if (queryComponent is IExpression)
                        {
                            IExpression expression = (IExpression)queryComponent;
                            Identifier currentIdentifier = null;
                            IQueryComponentNavigator queryComponentNavigator = expression.GetQueryComponentNavigator();
                            if (queryComponentNavigator != null)
                            {
                                currentIdentifier = (_mainFromComponent is Identifier ? (Identifier)_mainFromComponent :
                                    _visitor.Query.FindAllComponents<Identifier>().Where(item => _visitor.Query.RetrieveIdentifier(item.Name) == _visitor.ItemNameOverride).FirstOrDefault()) ?? _visitor.Query.Subject;
                                queryComponentNavigator.ReplaceComponent(Identifier.Current, currentIdentifier);
                            }

                            resultNavigator.AddComponent(new Filter(expression));
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(System.String.Format("Cannot add value of type '{0}' as an method call argument.", _from.GetType().FullName));
            }

            _visitor.ItemNameOverride = currentItemNameOverride;
            _visitor.ConstantFromClause = null;
        }

        /// <summary>Visits a where clause.</summary>
        /// <param name="whereClause">Where clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitWhereClause(WhereClause whereClause, Remotion.Linq.QueryModel queryModel, int index)
        {
            _visitor.VisitExpression(whereClause.Predicate);
            QueryComponent queryComponent = _visitor.RetrieveComponent();
            if (queryComponent != null)
            {
                _bodies.Add(queryComponent);
            }

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        /// <summary>Visits a main from clause.</summary>
        /// <param name="fromClause">Main from clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        public override void VisitMainFromClause(MainFromClause fromClause, Remotion.Linq.QueryModel queryModel)
        {
            _visitor.VisitExpression(fromClause.FromExpression);
            _from = _visitor.RetrieveComponent();
            base.VisitMainFromClause(fromClause, queryModel);
        }

        /// <summary>Visits an additional from clause.</summary>
        /// <param name="fromClause">From clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            _visitor.VisitExpression(fromClause.FromExpression);
            _from = _visitor.RetrieveComponent();
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
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

        private void VisitAnyResultOperator(AnyResultOperator anyResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            string targetIdentifierString = _visitor.Query.CreateIdentifier(_visitor.ItemNameOverride);
            IList<EntityConstrain> entityConstrains = null;
            StrongEntityAccessor entityAccessor = (from accessor in _visitor.Query.GetQueryComponentNavigator().FindAllComponents<StrongEntityAccessor>()
                                                   let constrains = accessor.Elements.OfType<EntityConstrain>()
                                                   from constrain in constrains
                                                   let predicate = constrain.Predicate as Literal
                                                   where predicate != null
                                                   let predicateUri = (Uri)predicate.Value
                                                   let identifier = constrain.Value as Identifier
                                                   where identifier != null
                                                   let identifierString = _visitor.Query.RetrieveIdentifier(identifier.Name)
                                                   where (identifierString == targetIdentifierString) || ((_fromExpression != null) && (constrain.TargetExpression.EqualsTo(_fromExpression)))
                                                   where (entityConstrains = constrains.Where(item =>
                                                       (item.Predicate is Literal) && (((Uri)((Literal)item.Predicate).Value).AbsoluteUri == predicateUri.AbsoluteUri)).ToList()).Count > 0
                                                   select accessor).FirstOrDefault();
            if (entityAccessor != null)
            {
                foreach (EntityConstrain entityConstrain in entityConstrains)
                {
                    int indexOf = entityAccessor.Elements.IndexOf(entityConstrain);
                    entityAccessor.Elements.RemoveAt(indexOf);
                    OptionalPattern optional = new OptionalPattern();
                    optional.Patterns.Add(entityConstrain);
                    entityAccessor.Elements.Insert(indexOf, optional);
                }
            }

            Call call = new Call(MethodNames.Bound);
            _result = call;
        }

        private void VisitContainsResultOperator(ContainsResultOperator containsResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            Call call = new Call(MethodNames.In);
            call.Arguments.Add(Identifier.Current);
            _result = call;
        }

        private void VisitCountResultOperator(CountResultOperator countResultOperator, Remotion.Linq.QueryModel queryModel, int index)
        {
            _result = new Call(MethodNames.Count);
        }
        #endregion
    }
}