using System;
using System.Linq;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Linq
{
    /// <summary>Converts LINQ query model to SPARQL abstraction.</summary>
    internal class EntityQueryModelVisitor:QueryModelVisitorBase
    {
        #region Fields
        private readonly IMappingsRepository _mappingsRepository;
        private EntityQueryVisitor _visitor;
        private Query _query;
        private EntityAccessor _mainFromComponent;
        private QueryComponent _result;
        #endregion

        #region Constructors
        /// <summary>Default constructor with mappings repository passed.</summary>
        /// <param name="mappingsRepository">Mappings repository to be used to resolve properties.</param>
        public EntityQueryModelVisitor(IMappingsRepository mappingsRepository):this(new Query(),mappingsRepository)
        {
        }

        internal EntityQueryModelVisitor(Query query,IMappingsRepository mappingsRepository)
        {
            _visitor=new EntityQueryVisitor(_query=(Query)(_result=query),_mappingsRepository=mappingsRepository);
        }
        #endregion

        #region Properties
        /// <summary>Gets a SPARQL abstraction model.</summary>
        public Query Query { get { return _query; } }

        /// <summary>Gets a resulting query.</summary>
        internal QueryComponent Result { get { return _result; } }
        #endregion

        #region Public methods
        /// <summary>Visits a query model.</summary>
        /// <param name="queryModel">Query model to be visited.</param>
        public override void VisitQueryModel(Remotion.Linq.QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this,queryModel);
        }

        /// <summary>Visits a select clause.</summary>
        /// <param name="selectClause">Select clause to be visited.</param>
        /// <param name="queryModel">Query model containing given select clause.</param>
        public override void VisitSelectClause(SelectClause selectClause,Remotion.Linq.QueryModel queryModel)
        {
            QuerySourceReferenceExpression querySource=(QuerySourceReferenceExpression)selectClause.Selector;
            _visitor.VisitExpression(querySource);
            _query=(Query)_visitor.RetrieveComponent();
            _mainFromComponent=_query.FindAllComponents<EntityAccessor>().Where(item => item.SourceExpression==querySource.ReferencedQuerySource).First();
            if (_query.Subject==null)
            {
                _query.Subject=_mainFromComponent.About;
                UnboundConstrain genericConstrain=new UnboundConstrain(new Identifier("s"),new Identifier("p"),new Identifier("o"));
                _mainFromComponent.Elements.Insert(0,genericConstrain);
                _query.Select.Add(genericConstrain);
            }

            queryModel.MainFromClause.Accept(this,queryModel);
            _query.Select.Add(_mainFromComponent);
            VisitBodyClauses(queryModel.BodyClauses,queryModel);
            VisitResultOperators(queryModel.ResultOperators,queryModel);
        }

        /// <summary>Visits a where clause.</summary>
        /// <param name="whereClause">Where clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitWhereClause(WhereClause whereClause,Remotion.Linq.QueryModel queryModel,int index)
        {
            _visitor.VisitExpression(whereClause.Predicate);
            QueryComponent queryComponent=_visitor.RetrieveComponent();
            if (queryComponent is Query)
            {
                _query.Elements.Add((Query)queryComponent);
            }
            else if (!_mainFromComponent.Elements.Contains(queryComponent))
            {
                Filter filter=new Filter((IExpression)queryComponent);
                if (!_mainFromComponent.Elements.Contains(filter))
                {
                    _mainFromComponent.Elements.Add(filter);
                }
            }

            base.VisitWhereClause(whereClause,queryModel,index);
        }

        /// <summary>Visits a main from clause.</summary>
        /// <param name="fromClause">Main from clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        public override void VisitMainFromClause(MainFromClause fromClause,Remotion.Linq.QueryModel queryModel)
        {
            if ((typeof(IQueryable).IsAssignableFrom(fromClause.FromExpression.Type))&&
                (fromClause.FromExpression.Type.GetGenericArguments().Length>0)&&
                (fromClause.FromExpression.Type.GetGenericArguments()[0]!=typeof(IEntity)))
            {
                IClassMapping classMapping=_mappingsRepository.FindClassMapping(fromClause.FromExpression.Type.GetGenericArguments()[0]);
                if (classMapping!=null)
                {
                    EntityConstrain constrain=new EntityConstrain(new Literal(RomanticWeb.Vocabularies.Rdf.Type),new Literal(classMapping.Uri));
                    if (!_mainFromComponent.Elements.Contains(constrain))
                    {
                        _mainFromComponent.Elements.Add(constrain);
                    }
                }
            }
            else if (fromClause.FromExpression is System.Linq.Expressions.MemberExpression)
            {
                System.Linq.Expressions.MemberExpression memberExpression=(System.Linq.Expressions.MemberExpression)fromClause.FromExpression;
                if (memberExpression.Member is PropertyInfo)
                {
                    _visitor.VisitExpression(memberExpression.Expression);
                }
            }

            base.VisitMainFromClause(fromClause,queryModel);
        }

        /// <summary>Visits a result operator.</summary>
        /// <param name="resultOperator">Result operator to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the visited result operator in the result operators collection.</param>
        public override void VisitResultOperator(ResultOperatorBase resultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
            MethodInfo visitResultOperatorMethod=GetType().GetMethod("Visit"+resultOperator.GetType().Name,BindingFlags.Instance|BindingFlags.NonPublic);
            if (visitResultOperatorMethod!=null)
            {
                visitResultOperatorMethod.Invoke(this,new object[] { resultOperator,queryModel,index });
            }

            base.VisitResultOperator(resultOperator,queryModel,index);
        }
        #endregion

        #region Non-public methods
        /// <summary>Visits body clauses.</summary>
        /// <param name="bodyClauses">Body clause to be visited.</param>
        /// <param name="queryModel">Query model containing given body clause.</param>
        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses,Remotion.Linq.QueryModel queryModel)
        {
            foreach (var indexValuePair in bodyClauses.AsChangeResistantEnumerableWithIndex())
            {
                indexValuePair.Value.Accept(this,queryModel,indexValuePair.Index);
            }
        }

        private void VisitAnyResultOperator(AnyResultOperator anyResultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
            if (_query.IsSubQuery)
            {
                Call call=new Call(MethodNames.Any);
                call.Arguments.Add(_query);
                _result=call;
            }
            else
            {
                _query.QueryForm=QueryForms.Ask;
            }
        }

        private void VisitContainsResultOperator(ContainsResultOperator containsResultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
            if (_query.IsSubQuery)
            {
                Call call=new Call(MethodNames.Any);
                call.Arguments.Add(_query);
                _result=call;
            }
            else
            {
                throw new NotSupportedException(System.String.Format("Cannot perform 'Contains' operation on top level query."));
            }
        }

        private void VisitCountResultOperator(CountResultOperator countResultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
            Call distinct=new Call(MethodNames.Distinct);
            distinct.Arguments.Add((_query.IsSubQuery?_mainFromComponent.About:_query.Select.Where(item => item is UnboundConstrain).Select(item => ((UnboundConstrain)item).Subject).First()));
            Call count=new Call(MethodNames.Count);
            count.Arguments.Add(distinct);
            _query.Select.Clear();
            Alias alias=new Alias(count,new Identifier(_query.CreateVariableName(_query.RetrieveIdentifier(_mainFromComponent.About.Name)+"Count")));
            _query.Select.Add(alias);
        }
        #endregion
    }
}