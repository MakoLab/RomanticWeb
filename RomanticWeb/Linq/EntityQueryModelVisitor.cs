using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Model.Navigators;
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
        private Identifier _subject;
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
            _subject=null;
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

            _subject=_query.Subject;

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
            IQueryComponentNavigator queryComponentNavigator=queryComponent.GetQueryComponentNavigator();
            if (queryComponentNavigator!=null)
            {
                queryComponentNavigator.ReplaceComponent(Identifier.Current,_subject);
            }

            if (queryComponent is QueryElement)
            {
                if ((!(queryComponent is EntityConstrain))&&(!_query.Elements.Contains((QueryElement)queryComponent)))
                {
                    _query.Elements.Add((QueryElement)queryComponent);
                }
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

        /// <summary>Visits an additional from clause.</summary>
        /// <param name="fromClause">From clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model.</param>
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause,QueryModel queryModel,int index)
        {
            VisitQuerableFromClause(fromClause,queryModel,index);
            EntityAccessor entityAccessor=_visitor.GetEntityAccessor(fromClause);
            if (entityAccessor!=null)
            {
                if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
                {
                    _query.Elements.Add(entityAccessor);
                }
            }
            else
            {
                NonEntityQueryModelVisitor modelVisitor=new NonEntityQueryModelVisitor(_visitor);
                modelVisitor.VisitAdditionalFromClause(fromClause,queryModel,index);
                if (modelVisitor.From is Identifier)
                {
                    _subject=(Identifier)modelVisitor.From;
                }
            }

            base.VisitAdditionalFromClause(fromClause,queryModel,index);
        }

        /// <summary>Visits a main from clause.</summary>
        /// <param name="fromClause">Main from clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        public override void VisitMainFromClause(MainFromClause fromClause,Remotion.Linq.QueryModel queryModel)
        {
            VisitQuerableFromClause(fromClause,queryModel,-1);
            if (fromClause.FromExpression is System.Linq.Expressions.MemberExpression)
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
            else
            {
                throw new NotSupportedException(System.String.Format("Expressions of type '{0}' are not supported.",resultOperator.GetType().Name.Replace("ResultOperator",System.String.Empty)));
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

        /// <summary>Visits a from clause.</summary>
        /// <param name="fromClause">From clause to be visited.</param>
        /// <param name="queryModel">Query model containing given from clause.</param>
        /// <param name="index">Index of the where clause in the query model. In case of the main from clause this value is -1.</param>
        protected virtual void VisitQuerableFromClause(FromClauseBase fromClause,Remotion.Linq.QueryModel queryModel,int index)
        {
            if ((typeof(IQueryable).IsAssignableFrom(fromClause.FromExpression.Type))&&
                (fromClause.FromExpression.Type.GetGenericArguments().Length>0)&&
                (fromClause.FromExpression.Type.GetGenericArguments()[0]!=typeof(IEntity)))
            {
                var classMappings=_mappingsRepository.FindClassMappings(fromClause.FromExpression.Type.GetGenericArguments()[0]);
                if (classMappings.Any())
                {
                    EntityConstrain constrain=new EntityConstrain(new Literal(Vocabularies.Rdf.Type),new Literal(classMappings.First().Uri));
                    if (!_mainFromComponent.Elements.Contains(constrain))
                    {
                        _mainFromComponent.Elements.Add(constrain);
                    }
                }
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

        private void VisitFirstResultOperator(FirstResultOperator countResultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
        }

        private void VisitSingleResultOperator(SingleResultOperator countResultOperator,Remotion.Linq.QueryModel queryModel,int index)
        {
        }
        #endregion
    }
}