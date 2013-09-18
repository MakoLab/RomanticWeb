using System;
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
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	public class EntityQueryVisitor:ThrowingExpressionTreeVisitor
	{
		private List<QuerySourceReferenceExpression> _expressionSources;
		private StringBuilder _commandText;
		private IOntologyProvider _ontologyProvider;
		private IMappingsRepository _mappingsRepository;
		private int _bracketsStack=0;
		private bool _suppressOperators=false;
		private int _currentGraphPattern=0;

		internal EntityQueryVisitor(StringBuilder commandText,List<QuerySourceReferenceExpression> expressionSources,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository):base()
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
		}

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

		protected string CreateCommadText()
		{
			return _commandText.ToString();
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			_expressionSources.Add(expression);
			return expression;
		}

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

		protected override Expression VisitUnaryExpression(UnaryExpression expression)
		{
			if (expression.NodeType==ExpressionType.Convert)
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
			else
			{
				return base.VisitUnaryExpression(expression);
			}

			return expression;
		}

		protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		{
			if ((expression.Method.Name=="get_Item")&&(expression.Arguments.Count==1))
			{
				if (expression.Method.DeclaringType.IsAssignableFrom(typeof(IEntity)))
				{
					MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.Method.DeclaringType });
					IMapping mapping=(IMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
					if ((mapping!=null)&&(mapping.Type!=null))
					{
						Ontology ontology=
							_ontologyProvider.Ontologies.Where(
								item => mapping.Type.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
						if (ontology!=null)
						{
							CurrentGraphPattern++;
							_commandText.AppendFormat("<{0}> ",mapping.Type.Uri.AbsoluteUri);
						}
						else
						{
							return base.VisitMethodCallExpression(expression);
						}
					}
				}
				else if (expression.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))
				{
					_commandText.AppendFormat("{0}:");
					CurrentGraphPattern++;
					VisitExpression(expression.Arguments[0]);
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

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if ((expression.Member.Name=="Id")&&(typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType)))
			{
				if ((CurrentGraphPattern==0)||(CurrentGraphPattern==3))
				{
					_commandText.AppendFormat("FILTER (?s{0} ",_expressionSources.IndexOf((QuerySourceReferenceExpression)expression.Expression));
					_bracketsStack++;
				}
				else
				{
					_suppressOperators=true;
				}
			}
			else
			{
				return base.VisitMemberExpression(expression);
			}

			return expression;
		}

		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			string value;
			switch (expression.Type.FullName)
			{
				case "System.SByte":
					value=System.String.Format("\"{0}\"^^xsd:byte",expression.Value);
					break;
				case "System.Byte":
					value=System.String.Format("\"{0}\"^^xsd:unsignedByte",expression.Value);
					break;
				case "System.Int16":
					value=System.String.Format("\"{0}\"^^xsd:short",expression.Value);
					break;
				case "System.UInt16":
					value=System.String.Format("\"{0}\"^^xsd:unsignedShort",expression.Value);
					break;
				case "System.Int32":
					value=System.String.Format("\"{0}\"^^xsd:int",expression.Value);
					break;
				case "System.UInt32":
					value=System.String.Format("\"{0}\"^^xsd:unsignedInt",expression.Value);
					break;
				case "System.Int64":
					value=System.String.Format("\"{0}\"^^xsd:long",expression.Value);
					break;
				case "System.UInt64":
					value=System.String.Format("\"{0}\"^^xsd:unsignedLong",expression.Value);
					break;
				case "System.Single":
					value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:float",expression.Value);
					break;
				case "System.Double":
					value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:double",expression.Value);
					break;
				case "System.Decimal":
					value=System.String.Format(CultureInfo.InvariantCulture,"\"{0}\"^^xsd:decimal",expression.Value);
					break;
				case "System.DateTme":
					value=((DateTime)expression.Value).ToString("\"yyyy\\-MM\\-dd\\THH:mm:ssZ\"^^xsd:dateTime");
					break;
				case "Sytem.Char":
				case "System.String":
					value=System.String.Format("\"{0}\"^^xsd:string",expression.Value);
					break;
				default:
					{
						if (typeof(EntityId).IsAssignableFrom(expression.Type))
						{
							value=System.String.Format("<{0}>",expression.Value);
						}
						else
						{
							value=System.String.Format("\"{0}\"",expression.Value);
						}

						break;
					}
			}

			_commandText.AppendFormat("{0} ",value);
			CurrentGraphPattern++;

			return expression;
		}

		protected override Expression VisitTypeBinaryExpression(TypeBinaryExpression expression)
		{
			if (expression.NodeType==ExpressionType.TypeIs)
			{
				if (!typeof(IEntity).IsAssignableFrom(expression.TypeOperand))
				{
					return base.VisitTypeBinaryExpression(expression);
				}

				MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.TypeOperand });
				IMapping mapping=(IMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
				if ((mapping!=null)&&(mapping.Type!=null))
				{
					Ontology ontology=
						_ontologyProvider.Ontologies.Where(
							item => mapping.Type.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
					if (ontology!=null)
					{
						_commandText.AppendFormat("?s{0} a <{1}>",0,mapping.Type.Uri.AbsoluteUri);
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

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			string subCommandText=EntityQueryModelVisitor.CreateCommandText(expression.QueryModel,_expressionSources,_ontologyProvider,_mappingsRepository);
			if (subCommandText.Length>0)
			{
				_commandText.AppendFormat("{{ {0} }}",subCommandText);
			}

			return expression;
		}

		protected override Exception CreateUnhandledItemException<T>(T unhandledItem,string visitMethod)
		{
			Expression expression=unhandledItem as Expression;
			return new NotSupportedException(expression!=null?FormattingExpressionTreeVisitor.Format(expression):unhandledItem.ToString());
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
				if (current is MethodCallExpression)
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

				result=new Tuple<string,string>("?s"+_expressionSources.IndexOf(querySource),"<"+ontology.BaseUri.AbsoluteUri+propertyName+">");
			}

			return result;
		}
	}
}