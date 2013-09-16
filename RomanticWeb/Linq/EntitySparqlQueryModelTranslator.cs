using System;
using System.Collections.Generic;
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
    using System.Linq;

    using RomanticWeb.Mapping;
    using RomanticWeb.Mapping.Model;

    internal class EntitySparqlQueryModelTranslator
	{
        private IMappingsRepository _mappings;

	    private IOntologyProvider _ontologyProvider;

	    internal EntitySparqlQueryModelTranslator(IEntityFactory entityFactory,IMappingsRepository mappings,IOntologyProvider ontologyProvider)
		{
	        _mappings=mappings;
	        _ontologyProvider=ontologyProvider;
		}

		internal string CreateCommandText(QueryModel queryModel)
		{
			if (queryModel.SelectClause==null)
			{
			    throw new ArgumentOutOfRangeException("There is no select clause associated with query.");
			}

			Dictionary<object,char> variableMap=new Dictionary<object,char>();
			string whereClause="\t?s ?p ?o."+Environment.NewLine;
			foreach (IBodyClause clause in queryModel.BodyClauses)
			{
				if (clause is WhereClause)
				{
				    whereClause+="\t"+TransformWhereClause(queryModel,(WhereClause)clause,ref variableMap)+Environment.NewLine;
				}
			}

			string result=System.String.Format(
@"CONSTRUCT {{ ?s ?p ?o }}
WHERE {{
{1}}}",
      Environment.NewLine,
      whereClause);
			return result;
		}

		internal string CreateSingleResultCommandText(QueryModel queryModel)
		{
			string result=CreateCommandText(queryModel);
			if (result.Length>0)
            {
                result=System.String.Format(
@"CONSTRUCT {{ ?s ?p ?o. }}
WHERE {{
	?s ?p ?o.
	{{ {1} LIMIT 1
	}} FILTER (?s=?_s)
}}",
   Environment.NewLine,
   result.Replace("\r",string.Empty).Replace("\n",System.Environment.NewLine+"\t\t").Replace("?s","?_s").Replace("CONSTRUCT { ?_s ?p ?o }","SELECT DISTINCT ?_s"));
            }

			return result;
		}

		private string TransformWhereClause(QueryModel queryModel,WhereClause whereClause,ref Dictionary<object,char> variableMap)
		{
			string result=string.Empty;
			MethodInfo transformMethodInfo=GetType().GetMethod("Transform"+whereClause.Predicate.GetType().Name,BindingFlags.NonPublic|BindingFlags.Instance);
			if (transformMethodInfo!=null)
			{
			    result=(string)transformMethodInfo.Invoke(this,new object[] { whereClause.Predicate });
			}
			else
			{
			    throw new InvalidOperationException("Unsupported where clause.");
			}

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
					if ((methodCall.Method.Name=="get_Item")&&(methodCall.Arguments.Count==1)
					    &&(methodCall.Arguments[0] is ConstantExpression)
					    &&((methodCall.Method.DeclaringType.IsAssignableFrom(typeof(IEntity)))
					       ||(methodCall.Method.DeclaringType.IsAssignableFrom(typeof(OntologyAccessor)))))
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
					if ((!(typeof(Entity).IsAssignableFrom(expression.Type)))&&(!(typeof(OntologyAccessor).IsAssignableFrom(expression.Type))))
					{
						callStack.Clear();
						break;
					}

					current=expression.Operand;
				}
				else if (current is QuerySourceReferenceExpression)
				{
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
			Stack<MethodCallExpression> callStack=BuildPropertyAccessorCallStack(expression);
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
			    result=new Tuple<string,string>("?s","<"+ontology.BaseUri.AbsoluteUri+propertyName+">");
			}

			return result;
		}

		private string TransformMethodUnaryExpression(UnaryExpression expression)
		{
			string result=string.Empty;
			if (expression.NodeType==ExpressionType.Convert)
			{
			    if (expression.Operand is MethodCallExpression)
			    {
			        Tuple<string,string> tuple=ConvertToPropertyAccessor(expression);
			        if (tuple!=null)
			        {
			            result=System.String.Format("{0} {1}",tuple.Item1,tuple.Item2);
			        }
			    }
			}
			else
			{
			    throw new InvalidOperationException("Unsupported unary expression.");
			}

			return result;
		}

		private string TransformMethodBinaryExpression(BinaryExpression expression)
		{
		    string result=string.Empty;
		    string operatorSymbol=string.Empty;
			switch (expression.Method.Name)
			{
				case "op_Equality": operatorSymbol="=="; 
                    break;
			}

			if (operatorSymbol.Length>0)
			{
			    if ((expression.Left is MemberExpression)&&(((MemberExpression)expression.Left).Member.Name=="Id")
			        &&(typeof(Entity).IsAssignableFrom(((MemberExpression)expression.Left).Member.DeclaringType))
			        &&(expression.Right is ConstantExpression))
			    {
			        result=System.String.Format("<{0}> ?p ?o",((ConstantExpression)expression.Right).Value.ToString());
			    }
			    else if ((expression.Right is MemberExpression)&&(((MemberExpression)expression.Right).Member.Name=="Id")
			             &&(typeof(Entity).IsAssignableFrom(((MemberExpression)expression.Right).Member.DeclaringType))
			             &&(expression.Left is ConstantExpression))
			    {
			        result=System.String.Format("<{0}> ?p ?o",((ConstantExpression)expression.Left).Value.ToString());
			    }			    
                else if ((expression.Left is UnaryExpression)&&(expression.Right is ConstantExpression))
                {
                    result=System.String.Format(
			            "{0} {1}",
                        TransformMethodUnaryExpression((UnaryExpression)expression.Left),
                        System.String.Format(((((ConstantExpression)expression.Right).Type.IsValueType)||(((ConstantExpression)expression.Right).Type==typeof(string))?"\"{0}\"":"<{0}"),((ConstantExpression)expression.Right).Value.ToString()));
                }
			    else if ((expression.Left is ConstantExpression)&&(expression.Right is UnaryExpression))
                {
                    result=System.String.Format(
			            "{0} {1}",
			            TransformMethodUnaryExpression((UnaryExpression)expression.Right),
			            System.String.Format(((((ConstantExpression)expression.Left).Type.IsValueType)||(((ConstantExpression)expression.Right).Type==typeof(string))?"\"{0}\"":"<{0}>"),((ConstantExpression)expression.Left).Value.ToString()));
                }
			    else
			    {
			        throw new InvalidOperationException("Unsupported binary expression.");
			    }
			}
			else
			{
			    throw new InvalidOperationException("Unsupported binary expression.");
			}

			return result;
		}

		private string TransformTypeBinaryExpression(TypeBinaryExpression expression)
		{
		    string result=string.Empty;
			if (expression.NodeType==ExpressionType.TypeIs)
			{
				if (!typeof(IEntity).IsAssignableFrom(expression.TypeOperand))
				{
				    ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
				}

				MethodInfo mappingForMethodInfo=_mappings.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { expression.TypeOperand });
				IMapping mapping=(IMapping)mappingForMethodInfo.Invoke(_mappings,null);
				if ((mapping!=null)&&(mapping.Type!=null))
				{
				    Ontology ontology=
				        _ontologyProvider.Ontologies.Where(
				            item => mapping.Type.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
				    if (ontology!=null)
				    {
				        result=System.String.Format("?s a <{0}>",mapping.Type.Uri.AbsoluteUri);
				    }
				    else
				    {
				        ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
				    }
				}
				else
				{
				    ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
				}
			}

			return result;
		}

		private void ThrowInvalidCastException(Type expectedType,Type foundType)
		{
			throw new InvalidCastException(System.String.Format("Expected '{0}' type, found '{1}'.",expectedType.FullName,foundType.FullName));
		}
	}
}