using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NullGuard;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Expressions;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Model.Navigators;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Linq
{
    /// <summary>Visits query expressions.</summary>
    public class EntityQueryVisitor:ThrowingExpressionTreeVisitor,IQueryVisitor
    {
        #region Fields
        private readonly IMappingsRepository _mappingsRepository;
        private readonly IBaseUriSelectionPolicy _baseUriSelectionPolicy;
        private Query _query;
        private Stack<IQueryComponentNavigator> _currentComponent;
        private QueryComponent _lastComponent;
        private string _itemNameOverride;
        #endregion

        #region Constructors
        /// <summary>Creates an instance of the query visitor.</summary>
        /// <param name="query">Query to be filled.</param>
        /// <param name="mappingsRepository">Mappings repository used to resolve strongly typed properties and types.</param>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy to resolve relative Uris.</param>
        internal EntityQueryVisitor(Query query,IMappingsRepository mappingsRepository,[AllowNull] IBaseUriSelectionPolicy baseUriSelectionPolicy):base()
        {
            _mappingsRepository=mappingsRepository;
            _baseUriSelectionPolicy=baseUriSelectionPolicy;
            _currentComponent=new Stack<IQueryComponentNavigator>();
            _query=query;
            _itemNameOverride=null;
        }
        #endregion

        #region Properties
        /// <summary>Gets an associated query.</summary>
        Query IQueryVisitor.Query { get { return _query; } }

        /// <summary>Gets the mappings repository.</summary>
        IMappingsRepository IQueryVisitor.MappingsRepository { get { return _mappingsRepository; } }

        /// <summary>Gets an associated query.</summary>
        internal Query Query { get { return _query; } }

        /// <summary>Gets or sets an item name to be used when creating entity accessors.</summary>
        internal string ItemNameOverride { get { return _itemNameOverride; } set { _itemNameOverride=value; } }

        /// <summary>Gets the mappings repository.</summary>
        internal IMappingsRepository MappingsRepository { get { return _mappingsRepository; } }
        #endregion

        #region Public methods
        /// <summary>Retrevies last visited and transformed query and prepares for next inspection.</summary>
        /// <returns>Query component visited or query itself.</returns>
        public QueryComponent RetrieveComponent()
        {
            CleanupComponent(_lastComponent);
            QueryComponent result=_lastComponent;
            _lastComponent=(_currentComponent.Count>0?(QueryComponent)_currentComponent.Pop():_query);
            return result;
        }
        #endregion

        #region Protected methods
        /// <summary>Visits a query source expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited.</returns>
        protected override System.Linq.Expressions.Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            EntityAccessor entityAccessor=this.GetEntityAccessor(GetSourceExpression(expression));
            if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
            {
                _query.Elements.Add(entityAccessor);
            }

            _lastComponent=_query;
            return expression;
        }

        /// <summary>Visits a binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitBinaryExpression(System.Linq.Expressions.BinaryExpression expression)
        {
            MethodNames operatorName;
            System.Linq.Expressions.ExpressionType expressionType=expression.NodeType;
            switch (expression.NodeType)
            {
                case System.Linq.Expressions.ExpressionType.OrElse:
                    expressionType=System.Linq.Expressions.ExpressionType.Or;
                    goto default;
                case System.Linq.Expressions.ExpressionType.AndAlso:
                    expressionType=System.Linq.Expressions.ExpressionType.And;
                    goto default;
                default:
                    string methodName=Enum.GetNames(typeof(MethodNames)).Where(item => item==expressionType.ToString()).FirstOrDefault();
                    if (!System.String.IsNullOrEmpty(methodName))
                    {
                        operatorName=(MethodNames)Enum.Parse(typeof(MethodNames),methodName);
                    }
                    else
                    {
                        return base.VisitBinaryExpression(expression);
                    }

                    break;
            }

            BinaryOperator binaryOperator=new BinaryOperator(operatorName);
            HandleComponent(binaryOperator);

            VisitExpression(expression.Left);
            CleanupComponent(_lastComponent);

            VisitExpression(expression.Right);
            CleanupComponent(_lastComponent);

            _lastComponent=binaryOperator;
            return expression;
        }

        /// <summary>Visits an unary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitUnaryExpression(System.Linq.Expressions.UnaryExpression expression)
        {
            MethodNames operatorName;
            System.Linq.Expressions.ExpressionType expressionType=expression.NodeType;
            switch (expression.NodeType)
            {
                default:
                    string methodName=Enum.GetNames(typeof(MethodNames)).Where(item => item==expressionType.ToString()).FirstOrDefault();
                    if (!System.String.IsNullOrEmpty(methodName))
                    {
                        operatorName=(MethodNames)Enum.Parse(typeof(MethodNames),methodName);
                    }
                    else
                    {
                        return base.VisitUnaryExpression(expression);
                    }

                    break;
            }

            UnaryOperator unaryOperator=new UnaryOperator(operatorName);
            HandleComponent(unaryOperator);

            VisitExpression(expression.Operand);
            CleanupComponent(_lastComponent);

            _lastComponent=unaryOperator;
            return expression;
        }

        /// <summary>Visits a method call expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitMethodCallExpression(System.Linq.Expressions.MethodCallExpression expression)
        {
            Call call=null;
            switch (expression.Method.Name)
            {
                case "StartsWith":
                case "EndsWith":
                case "Contains":
                case "Substring":
                    if (expression.Method.DeclaringType==typeof(string))
                    {
                        call=new Call((MethodNames)Enum.Parse(typeof(MethodNames),expression.Method.Name));
                    }
                    else
                    {
                        goto default;
                    }

                    break;
                case "IsMatch":
                    if (expression.Method.DeclaringType==typeof(Regex))
                    {
                        call=new Call(MethodNames.Regex);
                    }
                    else
                    {
                        goto default;
                    }

                    break;
                case "Is":
                    if ((expression.Method.DeclaringType==typeof(EntityExtensions))&&(expression.Arguments.Count==2)&&(expression.Arguments.Count==expression.Method.GetParameters().Length)&&
                        (expression.Arguments[1] is System.Linq.Expressions.ConstantExpression))
                    {
                        object objectValue=((System.Linq.Expressions.ConstantExpression)expression.Arguments[1]).Value;
                        if ((objectValue!=null)&&(typeof(IEnumerable).IsAssignableFrom(objectValue.GetType()))&&(objectValue.GetType().FindItemType()==typeof(EntityId)))
                        {
                            IEnumerable<EntityId> types=(IEnumerable<EntityId>)objectValue;
                            int count=types.Count();
                            if (count>0)
                            {
                                EntityAccessor entityAccessor=this.GetEntityAccessor(this.GetSourceExpression(expression.Arguments[0]));
                                if (entityAccessor!=null)
                                {
                                    EntityTypeConstrain constrain=new EntityTypeConstrain(types.Select(item => item.Uri).First(),(count>1?types.Skip(1).Select(item => item.Uri).ToArray():new Uri[0]));
                                    entityAccessor.Elements.Add(constrain);
                                    _lastComponent=constrain;
                                    return expression;
                                }
                            }
                        }
                        else
                        {
                            return base.VisitMethodCallExpression(expression);
                        }
                    }
                    else
                    {
                        goto default;
                    }

                    break;
                default:
                    return base.VisitMethodCallExpression(expression);
            }

            HandleComponent(call);
            foreach (System.Linq.Expressions.Expression argument in expression.Arguments)
            {
                VisitExpression(argument);
                CleanupComponent(_lastComponent);
            }

            _lastComponent=call;
            return expression;
        }

        /// <summary>Visits a member expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitMemberExpression(System.Linq.Expressions.MemberExpression expression)
        {
            if ((expression.Member.Name=="Id")&&(typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType)))
            {
                Remotion.Linq.Clauses.FromClauseBase target=GetMemberTarget(expression);
                if (target!=null)
                {
                    return VisitEntityId(new EntityIdentifierExpression(expression,target));
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.Member.DeclaringType);
                }
            }
            else if ((typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType))&&(expression.Member is PropertyInfo))
            {
                IPropertyMapping propertyMapping=_mappingsRepository.FindPropertyMapping((PropertyInfo)expression.Member);
                Remotion.Linq.Clauses.FromClauseBase target=GetMemberTarget(expression);
                if ((propertyMapping!=null)&&(target!=null))
                {
                    return VisitEntityProperty(new EntityPropertyExpression(expression,propertyMapping,target,(_itemNameOverride!=null?_itemNameOverride:expression.Member.Name)));
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.Member.DeclaringType);
                }
            }

            return base.VisitMemberExpression(expression);
        }

        /// <summary>Visits a constant expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitConstantExpression(System.Linq.Expressions.ConstantExpression expression)
        {
            if ((expression.Value is IEnumerable)&&(!(expression.Value is string)))
            {
                List list=new List();
                IEnumerable value=(IEnumerable)expression.Value;
                foreach (object item in value)
                {
                    list.Values.Add(new Literal(ResolveRelativeUriIfNecessery(item)));
                }

                _lastComponent=list;
            }
            else if (expression.Value is IEntity)
            {
                HandleComponent(_lastComponent=new Literal(((IEntity)expression.Value).Id.Uri));
            }
            else
            {
                HandleComponent(_lastComponent=new Literal(ResolveRelativeUriIfNecessery(expression.Value)));
            }

            return expression;
        }

        /// <summary>Visits a sub-query expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase sourceExpression=(Remotion.Linq.Clauses.FromClauseBase)((QuerySourceReferenceExpression)expression.QueryModel.SelectClause.Selector).ReferencedQuerySource;
            EntityAccessor entityAccessor=this.GetEntityAccessor(sourceExpression);
            if (entityAccessor!=null)
            {
                Query query=_query.CreateSubQuery(entityAccessor.About);
                if ((entityAccessor.OwnerQuery==null)&&(!query.Elements.Contains(entityAccessor)))
                {
                    query.Elements.Add(entityAccessor);
                }

                EntityQueryModelVisitor queryModelVisitor=new EntityQueryModelVisitor(query,_mappingsRepository,_baseUriSelectionPolicy);
                queryModelVisitor.VisitQueryModel(expression.QueryModel);
                HandleComponent(queryModelVisitor.Result);
                CleanupComponent(_lastComponent);
                _lastComponent=queryModelVisitor.Result;
            }
            else
            {
                NonEntityQueryModelVisitor queryModelVisitor=new NonEntityQueryModelVisitor(this);
                queryModelVisitor.VisitQueryModel(expression.QueryModel);
                _lastComponent=queryModelVisitor.Result;
            }

            return expression;
        }

        /// <summary>Visits an entity identifier member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityId(EntityIdentifierExpression expression)
        {
            if ((_currentComponent.Count>0)&&(_currentComponent.Peek() is BinaryOperatorNavigator))
            {
                HandleComponent(_query.Subject);
                BinaryOperator binaryOperator=((BinaryOperatorNavigator)_currentComponent.Peek()).NavigatedComponent;
                Filter filter=new Filter(binaryOperator);
                EntityAccessor entityAccessor=this.GetEntityAccessor(expression.Target);
                if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
                {
                    _query.Elements.Add(entityAccessor);
                }

                entityAccessor.Elements.Add(filter);
                _currentComponent.Push(filter.GetQueryComponentNavigator());
                _lastComponent=filter;
            }
            else
            {
                return base.VisitMemberExpression(expression.Expression);
            }

            return expression;
        }

        /// <summary>Visits an entity member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityProperty(EntityPropertyExpression expression)
        {
            EntityAccessor entityAccessor=this.GetEntityAccessor(expression.Target);
            if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
            {
                _query.Elements.Add(entityAccessor);
            }

            Identifier memberIdentifier=(_query.FindAllComponents<EntityAccessor>()
                .Where(item => item.SourceExpression.FromExpression==expression.Expression)
                .Select(item => item.About).FirstOrDefault())??(new Identifier(_query.CreateVariableName(expression.Name.CamelCase())));
            EntityConstrain constrain=new EntityConstrain(new Literal(expression.PropertyMapping.Uri),memberIdentifier);
            if (!entityAccessor.Elements.Contains(constrain))
            {
                entityAccessor.Elements.Add(constrain);
            }

            HandleComponent(memberIdentifier);
            _lastComponent=memberIdentifier;
            return expression;
        }

        /// <summary>Visits a type binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitTypeBinaryExpression(System.Linq.Expressions.TypeBinaryExpression expression)
        {
            var classMappings=_mappingsRepository.FindClassMapping(expression.TypeOperand);
            if (classMappings.Any())
            {
                Remotion.Linq.Clauses.FromClauseBase sourceExpression=GetSourceExpression(expression.Expression);
                EntityAccessor entityAccessor=_query.FindAllComponents<EntityAccessor>().Where(item => item.SourceExpression==sourceExpression).FirstOrDefault();
                if (entityAccessor==null)
                {
                    entityAccessor=this.GetEntityAccessor(sourceExpression);
                    _query.Elements.Add(entityAccessor);
                }

                EntityTypeConstrain typeConstrain=this.CreateTypeConstrain(expression.TypeOperand);
                _lastComponent=typeConstrain;
                if ((_currentComponent.Count>0)&&(_currentComponent.Peek() is BinaryOperatorNavigator))
                {
                    HandleComponent(typeConstrain);
                }
                else if (!entityAccessor.Elements.Contains(typeConstrain))
                {
                    entityAccessor.Elements.Add(typeConstrain);
                }
            }
            else
            {
                return base.VisitTypeBinaryExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a unhandled expression.</summary>
        /// <param name="unhandledItem">Expression beeing unhandled.</param>
        /// <param name="visitMethod">Visitor method.</param>
        /// <returns>Expression visited</returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem,string visitMethod)
        {
            System.Linq.Expressions.Expression expression=unhandledItem as System.Linq.Expressions.Expression;
            return new NotSupportedException(expression!=null?FormattingExpressionTreeVisitor.Format(expression):unhandledItem.ToString());
        }
        #endregion

        #region Private methods
        private void HandleComponent(QueryComponent component)
        {
            if (_currentComponent.Count>0)
            {
                if (_currentComponent.Peek().CanAddComponent(component))
                {
                    if (!_currentComponent.Peek().ContainsComponent(component))
                    {
                        _currentComponent.Peek().AddComponent(component);
                    }
                }
                else
                {
                    _currentComponent.Pop();
                }
            }

            IQueryComponentNavigator queryComponentNavigator=component.GetQueryComponentNavigator();
            if (queryComponentNavigator!=null)
            {
                _currentComponent.Push(queryComponentNavigator);
            }
        }

        private void CleanupComponent(QueryComponent component)
        {
            IQueryComponentNavigator queryComponentNavigator=_lastComponent.GetQueryComponentNavigator();
            if (queryComponentNavigator!=null)
            {
                while ((_currentComponent.Count>0)&&(!_currentComponent.Peek().Equals(queryComponentNavigator)))
                {
                    _currentComponent.Pop();
                }

                if (_currentComponent.Count>0)
                {
                    _currentComponent.Pop();
                }
            }
        }

        private Remotion.Linq.Clauses.FromClauseBase GetMemberTarget(System.Linq.Expressions.MemberExpression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase result=null;

            if (expression.Expression is System.Linq.Expressions.MemberExpression)
            {
                System.Linq.Expressions.MemberExpression memberExpression=(System.Linq.Expressions.MemberExpression)expression.Expression;
                if (memberExpression.Member is PropertyInfo)
                {
                    PropertyInfo propertyInfo=(PropertyInfo)memberExpression.Member;
                    string itemName=propertyInfo.Name.CamelCase();
                    EntityAccessor entityAccessor=_query.FindAllComponents<EntityAccessor>().Where(item => item.SourceExpression.FromExpression==memberExpression).FirstOrDefault();
                    if (entityAccessor!=null)
                    {
                        itemName=_query.RetrieveIdentifier(entityAccessor.About.Name);
                    }

                    result=new FromPropertyClause(itemName,propertyInfo.PropertyType.FindItemType(),memberExpression);
                }
                else
                {
                    throw CreateUnhandledItemException<System.Linq.Expressions.MemberExpression>(expression,"GetMemberTarget");
                }
            }
            else
            {
                result=GetSourceExpression(expression);
            }

            return result;
        }

        private Remotion.Linq.Clauses.FromClauseBase GetSourceExpression(System.Linq.Expressions.Expression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase result=null;
            System.Linq.Expressions.Expression currentExpression=expression;
            bool isChange=false;
            while (currentExpression!=null)
            {
                if (currentExpression is System.Linq.Expressions.MemberExpression)
                {
                    System.Linq.Expressions.MemberExpression memberExpression=(System.Linq.Expressions.MemberExpression)currentExpression;
                    if (!(memberExpression.Member is PropertyInfo))
                    {
                        throw CreateUnhandledItemException<System.Linq.Expressions.MemberExpression>(memberExpression,"GetSourceExpression");
                    }

                    currentExpression=memberExpression.Expression;
                    isChange=true;
                }
                else if (currentExpression is Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)
                {
                    result=(Remotion.Linq.Clauses.FromClauseBase)((Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)currentExpression).ReferencedQuerySource;
                    if (!typeof(IQueryable).IsAssignableFrom(result.FromExpression.Type))
                    {
                        Stack<IQueryComponentNavigator> currentComponent=_currentComponent;
                        _currentComponent=new Stack<IQueryComponentNavigator>();
                        VisitExpression(result.FromExpression);
                        _currentComponent=currentComponent;
                        isChange=true;
                    }

                    break;
                }

                if (isChange)
                {
                    isChange=false;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private object ResolveRelativeUriIfNecessery(object value)
        {
            if ((value!=null)&&(_baseUriSelectionPolicy!=null))
            {
                if (value is Uri)
                {
                    Uri uri=(Uri)value;
                    if (!uri.IsAbsoluteUri)
                    {
                        value=new Uri(_baseUriSelectionPolicy.SelectBaseUri(new EntityId(uri)),uri);
                    }
                }
                else if (value is EntityId)
                {
                    EntityId entityId=(EntityId)value;
                    if (!entityId.Uri.IsAbsoluteUri)
                    {
                        value=entityId.MakeAbsolute(_baseUriSelectionPolicy.SelectBaseUri(entityId));
                    }
                }
            }

            return value;
        }
        #endregion
    }
}