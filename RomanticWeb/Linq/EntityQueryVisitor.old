using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    /// <summary>Visits query expressions.</summary>
    public class EntityQueryVisitor:ThrowingExpressionTreeVisitor
    {
        #region Fields
        private IDictionary<QuerySourceReferenceExpression,string> _expressionSources;
        private StringBuilder _commandText;
        private IOntologyProvider _ontologyProvider;
        private IMappingsRepository _mappingsRepository;
        private int _bracketsStack=0;
        private bool _suppressOperators=false;
        private int _currentGraphPattern=0;
        private string _querySourceVariableName="?s0";
        #endregion

        #region Constructors
        /// <summary>Creates an instance of the query visitor.</summary>
        /// <param name="commandText">Command text string builder to be used to create query elements.</param>
        /// <param name="expressionSources">Enumeration of expression sources acknowledged when parsing parent query models.</param>
        /// <param name="querySourceVariableName">Query source variable name if any.</param>
        /// <param name="ontologyProvider">Ontology providerholding the data scheme.</param>
        /// <param name="mappingsRepository">Mappings repository used to resolve strongly typed properties and types.</param>
        internal EntityQueryVisitor(StringBuilder commandText,IDictionary<QuerySourceReferenceExpression,string> expressionSources,string querySourceVariableName,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository):base()
        {
            if (commandText==null)
            {
                throw new ArgumentNullException("commandText");
            }

            if (expressionSources==null)
            {
                throw new ArgumentNullException("expressionSources");
            }

            if (ontologyProvider==null)
            {
                throw new ArgumentNullException("ontologyProvider");
            }

            if (mappingsRepository==null)
            {
                throw new ArgumentNullException("mappingsRepository");
            }

            _ontologyProvider=ontologyProvider;
            _mappingsRepository=mappingsRepository;
            _expressionSources=expressionSources;
            _commandText=commandText;
            if (!System.String.IsNullOrEmpty(querySourceVariableName))
            {
                _querySourceVariableName=querySourceVariableName;
            }
        }
        #endregion

        #region Properties
        private int CurrentGraphPattern
        {
            get
            {
                return _currentGraphPattern;
            }

            set
            {
                if (value==3)
                {
                    if (_commandText[_commandText.Length-1]==' ')
                    {
                        _commandText.Remove(_commandText.Length-1,1);
                    }

                    _commandText.Append(".");
                }

                _currentGraphPattern=(value>3?1:value);
            }
        }
        #endregion

        #region Protected methods
        /// <summary>Creates a comman string from given expression.</summary>
        /// <returns></returns>
        protected string CreateCommadText()
        {
            return _commandText.ToString();
        }

        /// <summary>Visits a query source expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            _expressionSources[expression]=_querySourceVariableName;
            return expression;
        }

        /// <summary>Visits a binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            _suppressOperators=false;
            VisitExpression(expression.Left);
            if (!_suppressOperators)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Equal:
                        _commandText.Append("=");
                        break;
                    case ExpressionType.NotEqual:
                        _commandText.Append("!=");
                        break;
                    case ExpressionType.GreaterThan:
                        _commandText.Append(">");
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        _commandText.Append(">=");
                        break;
                    case ExpressionType.LessThan:
                        _commandText.Append("<");
                        break;
                    case ExpressionType.LessThanOrEqual:
                        _commandText.Append("<=");
                        break;
                    case ExpressionType.AndAlso:
                    case ExpressionType.And:
                        _commandText.Append("&&");
                        break;
                    case ExpressionType.OrElse:
                    case ExpressionType.Or:
                        _commandText.Append("||");
                        break;
                    case ExpressionType.Not:
                        _commandText.Append("!");
                        break;
                    case ExpressionType.Add:
                        _commandText.Append("+");
                        break;
                    case ExpressionType.Subtract:
                        _commandText.Append("-");
                        break;
                    case ExpressionType.Multiply:
                        _commandText.Append("*");
                        break;
                    case ExpressionType.Divide:
                        _commandText.Append("/");
                        break;
                    case ExpressionType.Modulo:
                        _commandText.Append("%");
                        break;
                    default:
                        base.VisitBinaryExpression(expression);
                        break;
                }
            }

            if ((_commandText.Length>0)&&(_commandText[_commandText.Length-1]!=' '))
            {
                _commandText.Append(" ");
            }
    
            VisitExpression(expression.Right);
            CloseBrackets();
            _suppressOperators=false;
            return expression;
        }

        /// <summary>Visits an unary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            if (expression.NodeType==ExpressionType.Convert)
            {
                if (typeof(EntityId).IsAssignableFrom(expression.Type))
                {
                    _suppressOperators=true;
                    Tuple<string,string> propertyAccessor=ConvertToPropertyAccessor(expression);
                    if (propertyAccessor!=null)
                    {
                        _commandText.AppendFormat("{0}",propertyAccessor.Item2);
                    }

                    return expression;
                }
                else
                {
                    Tuple<string,string> propertyAccessor=ConvertToPropertyAccessor(expression);
                    if (propertyAccessor!=null)
                    {
                        _suppressOperators=true;
                        _commandText.AppendFormat("{0} {1} ",propertyAccessor.Item1,propertyAccessor.Item2);
                        CurrentGraphPattern=2;
                    }
                    else
                    {
                        return base.VisitUnaryExpression(expression);
                    }
                }
            }
            else
            {
                return base.VisitUnaryExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a method call expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            if ((expression.Method.Name=="get_Item")&&(expression.Arguments.Count==1))
            {
                if (expression.Method.DeclaringType.IsAssignableFrom(typeof(IEntity)))
                {
                    MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.Method.DeclaringType });
                    IEntityMapping entityMapping=(IEntityMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
                    if ((entityMapping!=null)&&(entityMapping.Class!=null))
                    {
                        Ontology ontology=
                            _ontologyProvider.Ontologies.Where(
                                item => entityMapping.Class.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
                        if (ontology!=null)
                        {
                            CurrentGraphPattern++;
                            _commandText.AppendFormat("<{0}> ",entityMapping.Class.Uri.AbsoluteUri);
                        }
                        else
                        {
                            return base.VisitMethodCallExpression(expression);
                        }
                    }
                }
                else if (expression.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))
                {
                    CurrentGraphPattern++;
                    VisitExpression(expression.Arguments[0]);
                    _commandText.Append(":");
                }
                else
                {
                    return base.VisitMethodCallExpression(expression);
                }
            }
            else
            {
                return base.VisitMethodCallExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a member expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if ((expression.Member.Name=="Id")&&(typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType)))
            {
                if ((CurrentGraphPattern==0)||(CurrentGraphPattern==3))
                {
                    _commandText.AppendFormat("FILTER ({0} ",_expressionSources.Where(item => item.Key==(QuerySourceReferenceExpression)expression.Expression).Select(item => item.Value).First());
                    _bracketsStack++;
                }
                else
                {
                    _suppressOperators=true;
                }
            }
            else if (typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType))
            {
                MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.Member.DeclaringType });
                IEntityMapping entityMapping=(IEntityMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
                if (entityMapping!=null)
                {
                    IPropertyMapping propertyMapping=entityMapping.PropertyFor(expression.Member.Name);
                    if (propertyMapping!=null)
                    {
                        _commandText.AppendFormat("?s{0} <{1}> ?o{0} ",0,propertyMapping.Uri.AbsoluteUri);
                        CurrentGraphPattern=3;
                    }
                    else
                    {
                        ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.Member.DeclaringType);
                    }
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.Member.DeclaringType);
                }
            }
            else
            {
                return base.VisitMemberExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a constant expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            string value=ConvertConstant(expression.Value);
            if (value!=null)
            {
                _commandText.AppendFormat("{0} ",value);
                CurrentGraphPattern++;
            }

            return expression;
        }

        /// <summary>Visits a type binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitTypeBinaryExpression(TypeBinaryExpression expression)
        {
            if (expression.NodeType==ExpressionType.TypeIs)
            {
                if (!typeof(IEntity).IsAssignableFrom(expression.TypeOperand))
                {
                    return base.VisitTypeBinaryExpression(expression);
                }

                MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.TypeOperand });
                IEntityMapping entityMapping=(IEntityMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
                if ((entityMapping!=null)&&(entityMapping.Class!=null))
                {
                    Ontology ontology=
                        _ontologyProvider.Ontologies.Where(
                            item => entityMapping.Class.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
                    if (ontology!=null)
                    {
                        _commandText.AppendFormat("?s{0} a <{1}>",0,entityMapping.Class.Uri.AbsoluteUri);
                        CurrentGraphPattern=3;
                    }
                    else
                    {
                        ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
                    }
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
                }
            }
            else
            {
                return base.VisitTypeBinaryExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a sub-query expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            string subCommandText=EntityQueryModelVisitor.CreateCommandText(expression.QueryModel,_expressionSources,null,_ontologyProvider,_mappingsRepository);
            if (subCommandText.Length>0)
            {
                _commandText.AppendFormat("{{ {0} }}",subCommandText);
            }

            return expression;
        }

        /// <summary>Visits a unhandled expression.</summary>
        /// <param name="unhandledItem">Expression beeing unhandled.</param>
        /// <param name="visitMethod">Visitor method.</param>
        /// <returns>Expression visited</returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem,string visitMethod)
        {
            Expression expression=unhandledItem as Expression;
            return new NotSupportedException(expression!=null?FormattingExpressionTreeVisitor.Format(expression):unhandledItem.ToString());
        }
        #endregion

        #region Private methods
        private string ConvertConstant(object constant)
        {
            string value=null;
            if (constant!=null)
            {
                switch (constant.GetType().FullName)
                {
                    case "System.SByte":
                        value=System.String.Format("\"{0}\"^^xsd:byte",constant);
                        break;
                    case "System.Byte":
                        value=System.String.Format("\"{0}\"^^xsd:unsignedByte",constant);
                        break;
                    case "System.Int16":
                        value=System.String.Format("\"{0}\"^^xsd:short",constant);
                        break;
                    case "System.UInt16":
                        value=System.String.Format("\"{0}\"^^xsd:unsignedShort",constant);
                        break;
                    case "System.Int32":
                        value=System.String.Format("\"{0}\"^^xsd:int",constant);
                        break;
                    case "System.UInt32":
                        value=System.String.Format("\"{0}\"^^xsd:unsignedInt",constant);
                        break;
                    case "System.Int64":
                        value=System.String.Format("\"{0}\"^^xsd:long",constant);
                        break;
                    case "System.UInt64":
                        value=System.String.Format("\"{0}\"^^xsd:unsignedLong",constant);
                        break;
                    case "System.Single":
                        value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:float",constant);
                        break;
                    case "System.Double":
                        value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:double",constant);
                        break;
                    case "System.Decimal":
                        value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:decimal",constant);
                        break;
                    case "System.DateTme":
                        value=((DateTime)constant).ToString("\"yyyy\\-MM\\-dd\\THH:mm:ssZ\"^^xsd:dateTime");
                        break;
                    case "Sytem.Char":
                    case "System.String":
                        value=System.String.Format("\"{0}\"^^xsd:string",constant);
                        break;
                    default:
                        {
                            if (typeof(EntityId).IsAssignableFrom(constant.GetType()))
                            {
                                value=System.String.Format("<{0}>",constant);
                            }
                            else if (typeof(IEnumerable).IsAssignableFrom(constant.GetType()))
                            {
                                value=System.String.Empty;
                                foreach (object item in (IEnumerable)constant)
                                {
                                    string itemValue=ConvertConstant(item);
                                    if (itemValue!=null)
                                    {
                                        value=System.String.Format("{0}, {1}",value,itemValue);
                                    }
                                }

                                if (value.Length>0)
                                {
                                    value=value.Substring(2);
                                }
                            }
                            else
                            {
                                value=System.String.Format("\"{0}\"",constant);
                            }

                            break;
                        }
                }
            }

            return value;
        }

        private void CloseBrackets()
        {
            while (_bracketsStack>0)
            {
                _commandText.Append(")");
                _bracketsStack--;
            }
        }

        private Stack<MethodCallExpression> BuildPropertyAccessorCallStack(UnaryExpression expression,out QuerySourceReferenceExpression querySource)
        {
            Stack<MethodCallExpression> callStack=new Stack<MethodCallExpression>();
            querySource=null;
            Expression current=expression.Operand;
            int index=11;
            do
            {
                index--;
                if ((current is PartialEvaluationExceptionExpression)&&(((PartialEvaluationExceptionExpression)current).EvaluatedExpression is MethodCallExpression))
                {
                    MethodCallExpression methodCall=(MethodCallExpression)((PartialEvaluationExceptionExpression)current).EvaluatedExpression;
                    if ((methodCall.Method.Name=="get_Item")&&
                        (methodCall.Arguments.Count==1)&&
                        (methodCall.Arguments[0] is ConstantExpression)&&(
                            (methodCall.Method.DeclaringType.IsAssignableFrom(typeof(IEntity)))||
                            (methodCall.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))))
                    {
                        callStack.Push(methodCall);
                        current=methodCall.Object;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (current is MethodCallExpression)
                {
                    MethodCallExpression methodCall=(MethodCallExpression)current;
                    if ((methodCall.Method.Name=="get_Item")&&
                        (methodCall.Arguments.Count==1)&&
                        (methodCall.Arguments[0] is ConstantExpression)&&(
                            (methodCall.Method.DeclaringType.IsAssignableFrom(typeof(IEntity)))||
                            (methodCall.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))))
                    {
                        callStack.Push(methodCall);
                        current=methodCall.Object;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((current is UnaryExpression)&&(((UnaryExpression)current).NodeType==ExpressionType.Convert))
                {
                    expression=(UnaryExpression)current;
                    if ((!(typeof(IEntity).IsAssignableFrom(expression.Type)))&&(!(typeof(OntologyAccessor).IsAssignableFrom(expression.Type))))
                    {
                        callStack.Clear();
                        break;
                    }

                    current=expression.Operand;
                }
                else if (current is ConstantExpression)
                {
                    ConstantExpression constant=(ConstantExpression)current;
                    if (constant.Value is OntologyAccessor)
                    {
                        OntologyAccessor ontologyAccessor=(OntologyAccessor)constant.Value;
                        current=querySource=_expressionSources.Where(item => typeof(IEntity).IsAssignableFrom(item.Key.ReferencedQuerySource.ItemType)).Select(item => item.Key).FirstOrDefault();
                        callStack.Push(Expression.Call(constant,constant.Value.GetType().GetMethod("get_Item"),Expression.Constant(ontologyAccessor.Ontology.Prefix)));
                    }
                    else
                    {
                        break;
                    }
                }
                else if (current is QuerySourceReferenceExpression)
                {
                    querySource=(QuerySourceReferenceExpression)current;
                    break;
                }
                else
                {
                    callStack.Clear();
                    break;
                }
            }
            while (index>0);
            return callStack;
        }

        private Tuple<string,string> ConvertToPropertyAccessor(UnaryExpression expression)
        {
            Tuple<string,string> result=null;
            QuerySourceReferenceExpression querySource;
            Stack<MethodCallExpression> callStack=BuildPropertyAccessorCallStack(expression,out querySource);
            Ontology ontology=null;
            string propertyName=null;
            switch (callStack.Count)
            {
                case 1:
                {
                    propertyName=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
                    ontology=_ontologyProvider.Ontologies.Where(item => item.Properties.Any(property => property.PropertyName==propertyName)).FirstOrDefault();
                    break;
                }

                case 2:
                {
                    string prefix=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
                    propertyName=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
                    ontology=_ontologyProvider.Ontologies.Where(item => (item.Prefix==prefix)&&(item.Properties.Any(property => property.PropertyName==propertyName))).FirstOrDefault();
                    break;
                }
            }

            if (ontology!=null)
            {
                // AlignQuerySourceIndex(querySource);
                result=new Tuple<string,string>(_expressionSources.Where(item => item.Key==querySource).Select(item => item.Value).First(),"<"+ontology.BaseUri.AbsoluteUri+propertyName+">");
            }

            return result;
        }

        /*private void AlignQuerySourceIndex(QuerySourceReferenceExpression querySource)
        {
            int querySourceIndex=_expressionSources.IndexOf(querySource);
            string currentQuerySourceIndexString=_commandText.ToString().Substring("SELECT ?s".Length);
            currentQuerySourceIndexString=currentQuerySourceIndexString.Substring(0,currentQuerySourceIndexString.IndexOf(" "));
            int currentQuerySourceIndex=Int32.Parse(currentQuerySourceIndexString);
            if (querySourceIndex!=currentQuerySourceIndex)
            {
                Regex querySourceRegularExpression=new Regex("\\?(?<element>s|p|o)(?<currentQuerySourceIndex>[0-9]+)");
                Match match=querySourceRegularExpression.Match(_commandText.ToString(),0);
                while ((match!=null)&&(match.Groups["currentQuerySourceIndex"]!=null)&&(match.Groups["currentQuerySourceIndex"].Value.Length>0))
                {
                    _commandText.Remove(match.Groups["currentQuerySourceIndex"].Index,match.Groups["currentQuerySourceIndex"].Length).Insert(
                        match.Groups["currentQuerySourceIndex"].Index,querySourceIndex.ToString());
                    match=querySourceRegularExpression.Match(_commandText.ToString(),match.Groups["currentQuerySourceIndex"].Index);
                }
            }
        }*/
        #endregion
    }
}