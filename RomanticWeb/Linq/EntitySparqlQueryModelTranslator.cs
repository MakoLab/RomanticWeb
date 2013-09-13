using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	internal class EntitySparqlQueryModelTranslator
	{
		private IEntityFactory _entityFactory;

		internal EntitySparqlQueryModelTranslator(IEntityFactory entityFactory)
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
			_entityFactory=entityFactory;
		}

		internal string CreateCommandText(QueryModel queryModel)
		{
			if (queryModel==null)
				throw new ArgumentNullException("queryModel");
			if (queryModel.SelectClause==null)
				throw new ArgumentOutOfRangeException("There is no select clause associated with query.");
			Dictionary<object,char> variableMap=new Dictionary<object,char>();
			string whereClause="?s ?p ?o.";
			foreach (IBodyClause clause in queryModel.BodyClauses)
			{
				if (clause is WhereClause)
					whereClause+=" "+TransformWhereClause(queryModel,(WhereClause)clause,ref variableMap);
			}
			string result=System.String.Format("CONSTRUCT {{ ?s ?p ?o }} WHERE {{ {0} }}",whereClause);
			return result;
		}

		private string TransformWhereClause(QueryModel queryModel,WhereClause whereClause,ref Dictionary<object,char> variableMap)
		{
			string result="";
			MethodInfo transformMethodInfo=GetType().GetMethod("Transform"+whereClause.Predicate.GetType().Name,BindingFlags.NonPublic|BindingFlags.Instance);
			if (transformMethodInfo!=null)
				result=(string)transformMethodInfo.Invoke(this,new object[] { whereClause.Predicate });
			else
				throw new ArgumentNullException("Unsupported where clause.");
			return result;
		}

		private Stack<MethodCallExpression> BuildPropertyAccessorCallStack(UnaryExpression expression)
		{
			Stack<MethodCallExpression> callStack=new Stack<MethodCallExpression>();
			Expression current=expression.Operand;
			int index=11;
			do
			{
				index--;
				if (current is MethodCallExpression)
				{
					MethodCallExpression methodCall=(MethodCallExpression)current;
					if ((methodCall.Method.Name=="get_Item")&&(methodCall.Arguments.Count==1)&&(methodCall.Arguments[0] is ConstantExpression)&&
						((methodCall.Method.DeclaringType.IsAssignableFrom(typeof(Entity)))||(methodCall.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))))
					{
						callStack.Push(methodCall);
						current=methodCall.Object;
					}
					else
						break;
				}
				else if ((current is UnaryExpression)&&(((UnaryExpression)current).NodeType==ExpressionType.Convert))
				{
					expression=(UnaryExpression)current;
					if ((!(typeof(Entity).IsAssignableFrom(expression.Type)))&&(!(typeof(OntologyAccessor).IsAssignableFrom(expression.Type))))
					{
						callStack.Clear();
						break;
					}
					current=expression.Operand;
				}
				else if (current is QuerySourceReferenceExpression)
					break;
				else
				{
					callStack.Clear();
					break;
				}
			} while (index>0);
			return callStack;
		}

		private Tuple<string,string> ConvertToPropertyAccessor(UnaryExpression expression)
		{
			Tuple<string,string> result=null;
			Stack<MethodCallExpression> callStack=BuildPropertyAccessorCallStack(expression);
			Ontology ontology=null;
			string propertyName=null;
			switch (callStack.Count)
			{
				case 1:
				{
					propertyName=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
					ontology=_entityFactory.OntologyProvider.Ontologies.Where(item => item.Properties.Any(property => property.PropertyName==propertyName)).FirstOrDefault();
					break;
				}
				case 2:
				{
					string prefix=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
					propertyName=((ConstantExpression)callStack.Pop().Arguments[0]).Value.ToString();
					ontology=_entityFactory.OntologyProvider.Ontologies.Where(item => (item.Prefix==prefix)&&(item.Properties.Any(property => property.PropertyName==propertyName))).FirstOrDefault();
					break;
				}
			}
			if (ontology!=null)
				result=new Tuple<string,string>("?s","<"+ontology.BaseUri.AbsoluteUri+propertyName+">");
			return result;
		}

		private string TransformMethodUnaryExpression(UnaryExpression expression)
		{
			string result="";
			if (expression.NodeType==ExpressionType.Convert)
			{
				if (expression.Operand is MethodCallExpression)
				{
					Tuple<string,string> tuple=ConvertToPropertyAccessor(expression);
					if (tuple!=null)
						result=System.String.Format("{0} {1}",tuple.Item1,tuple.Item2);
				}
			}
			else
				throw new ArgumentOutOfRangeException("Unsupported unary expression.");
			return result;
		}

		private string TransformMethodBinaryExpression(BinaryExpression expression)
		{
			string result="";
			string operatorSymbol="";
			switch (expression.Method.Name)
			{
				case "op_Equality": operatorSymbol="=="; break;
			}
			if (operatorSymbol.Length>0)
			{
				if ((expression.Left is MemberExpression)&&(((MemberExpression)expression.Left).Member.Name=="Id")&&
					(typeof(Entity).IsAssignableFrom(((MemberExpression)expression.Left).Member.DeclaringType))&&(expression.Right is ConstantExpression))
					result=System.String.Format("<{0}> ?p ?o",((ConstantExpression)expression.Right).Value.ToString());
				else if ((expression.Right is MemberExpression)&&(((MemberExpression)expression.Right).Member.Name=="Id")&&
					(typeof(Entity).IsAssignableFrom(((MemberExpression)expression.Right).Member.DeclaringType))&&(expression.Left is ConstantExpression))
					result=System.String.Format("<{0}> ?p ?o",((ConstantExpression)expression.Left).Value.ToString());
				else if ((expression.Left is UnaryExpression)&&(expression.Right is ConstantExpression))
					result=System.String.Format("{0} {1}",TransformMethodUnaryExpression((UnaryExpression)expression.Left),System.String.Format(
						((((ConstantExpression)expression.Right).Type.IsValueType)||(((ConstantExpression)expression.Right).Type==typeof(string))?"\"{0}\"":"<{0}"),((ConstantExpression)expression.Right).Value.ToString()));
				else if ((expression.Left is ConstantExpression)&&(expression.Right is UnaryExpression))
					result=System.String.Format("{0} {1}",TransformMethodUnaryExpression((UnaryExpression)expression.Right),System.String.Format(
						((((ConstantExpression)expression.Left).Type.IsValueType)||(((ConstantExpression)expression.Right).Type==typeof(string))?"\"{0}\"":"<{0}>"),((ConstantExpression)expression.Left).Value.ToString()));
				else
					throw new ArgumentNullException("Unsupported binary expression.");
			}
			else
				throw new ArgumentNullException("Unsupported binary expression.");
			return result;
		}
	}
}