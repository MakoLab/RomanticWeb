using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
    public class EntityQueryVisitor:ThrowingExpressionTreeVisitor
    {
        #region Fields
        private Query _query;
        private IMappingsRepository _mappingsRepository;
        private Stack<IQueryComponentNavigator> _currentComponent;
        private QueryComponent _lastComponent;
        #endregion

        #region Constructors
        /// <summary>Creates an instance of the query visitor.</summary>
        /// <param name="query">Query to be filled.</param>
        /// <param name="mappingsRepository">Mappings repository used to resolve strongly typed properties and types.</param>
        internal EntityQueryVisitor(Query query,IMappingsRepository mappingsRepository):base()
        {
            _mappingsRepository=mappingsRepository;
            _currentComponent=new Stack<IQueryComponentNavigator>();
            _query=query;
        }
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
            EntityAccessor entityAccessor=GetEntityAccessor(GetSourceExpression(expression));
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
                    return VisitEntityProperty(new EntityPropertyExpression(expression,propertyMapping,target));
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
            HandleComponent(_lastComponent=new Literal(expression.Value));
            return expression;
        }

        /// <summary>Visits a sub-query expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            EntityAccessor entityAccessor=GetEntityAccessor((Remotion.Linq.Clauses.FromClauseBase)((QuerySourceReferenceExpression)expression.QueryModel.SelectClause.Selector).ReferencedQuerySource);
            Query query=_query.CreateSubQuery(entityAccessor.About);
            if ((entityAccessor.OwnerQuery==null)&&(!query.Elements.Contains(entityAccessor)))
            {
                query.Elements.Add(entityAccessor);
            }

            EntityQueryModelVisitor queryModelVisitor=new EntityQueryModelVisitor(query,_mappingsRepository);
            queryModelVisitor.VisitQueryModel(expression.QueryModel);
            HandleComponent(queryModelVisitor.Result);
            CleanupComponent(_lastComponent);
            _lastComponent=queryModelVisitor.Result;
            return expression;
        }

        /// <summary>Visits an entity identifier member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityId(EntityIdentifierExpression expression)
        {
            BinaryOperator binaryOperator=new BinaryOperator(MethodNames.Equal);
            binaryOperator.LeftOperand=_query.Subject;
            Filter filter=new Filter(binaryOperator);
            EntityAccessor entityAccessor=GetEntityAccessor(expression.Target);
            if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
            {
                _query.Elements.Add(entityAccessor);
            }

            entityAccessor.Elements.Add(filter);
            HandleComponent(binaryOperator);
            _lastComponent=binaryOperator;
            return expression;
        }

        /// <summary>Visits an entity member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityProperty(EntityPropertyExpression expression)
        {
            EntityAccessor entityAccessor=GetEntityAccessor(expression.Target);
            if ((entityAccessor.OwnerQuery==null)&&(!_query.Elements.Contains(entityAccessor)))
            {
                _query.Elements.Add(entityAccessor);
            }

            Identifier memberIdentifier=(Helpers.FindAllComponents<EntityAccessor>(_query)
                .Where(item => item.SourceExpression.FromExpression==expression.Expression)
                .Select(item => item.About).FirstOrDefault())??(new Identifier(_query.CreateVariableName(expression.EntityProperty.Name.CamelCase())));
            EntityConstrain constrain=new EntityConstrain(new Literal(expression.PropertyMapping.Uri),memberIdentifier);
            if (!entityAccessor.Elements.Contains(constrain))
            {
                entityAccessor.Elements.Add(constrain);
            }

            HandleComponent(memberIdentifier);
            _lastComponent=memberIdentifier;
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

            IQueryComponentNavigator queryComponentNavigator=Helpers.GetQueryComponentNavigator(component);
            if (queryComponentNavigator!=null)
            {
                _currentComponent.Push(queryComponentNavigator);
            }
        }

        private void CleanupComponent(QueryComponent component)
        {
            IQueryComponentNavigator queryComponentNavigator=Helpers.GetQueryComponentNavigator(_lastComponent);
            if (queryComponentNavigator!=null)
            {
                while ((_currentComponent.Count>0)&&(_currentComponent.Peek()!=queryComponentNavigator))
                {
                    _currentComponent.Pop();
                }

                if (_currentComponent.Count>0)
                {
                    _currentComponent.Pop();
                }
            }
        }

        private EntityAccessor GetEntityAccessor(Remotion.Linq.Clauses.FromClauseBase sourceExpression)
        {
            EntityAccessor entityAccessor=Helpers.FindAllComponents<EntityAccessor>(_query).Where(item => item.SourceExpression.FromExpression==sourceExpression.FromExpression).FirstOrDefault();
            if (entityAccessor==null)
            {
                entityAccessor=new EntityAccessor(new Identifier(_query.CreateVariableName(sourceExpression.ItemName.CamelCase())),sourceExpression);
                Type entityType=sourceExpression.ItemType.FindEntityType();
                if (entityType!=null)
                {
                    IClassMapping classMapping=_mappingsRepository.FindClassMapping(entityType);
                    if (classMapping!=null)
                    {
                        EntityConstrain constrain=new EntityConstrain(new Literal(RomanticWeb.Vocabularies.Rdf.Type),new Literal(classMapping.Uri));
                        if (!entityAccessor.Elements.Contains(constrain))
                        {
                            entityAccessor.Elements.Add(constrain);
                        }
                    }
                }
            }

            return entityAccessor;
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
                    EntityAccessor entityAccessor=Helpers.FindAllComponents<EntityAccessor>(_query).Where(item => item.SourceExpression.FromExpression==memberExpression).FirstOrDefault();
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
                    }

                    break;
                }
            }

            return result;
        }
        #endregion
    }
}