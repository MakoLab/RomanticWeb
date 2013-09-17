using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
    internal class EntitySparqlQueryModelTranslator
	{
		#region Fields
		private static readonly string ConstructTemplate=
@"CONSTRUCT {{ ?s ?p ?o }}
WHERE {{
{1}}}";

		private static readonly string SingleConstructTemplate=
@"CONSTRUCT {{ ?s ?p ?o. }}
WHERE {{
	?s ?p ?o.
	{{ {1} LIMIT 1
	}} FILTER (?s=?_s)
}}";

        private IMappingsRepository _mappings;

	    private IOntologyProvider _ontologyProvider;
		#endregion

		#region Constructors
		internal EntitySparqlQueryModelTranslator(IMappingsRepository mappings,IOntologyProvider ontologyProvider)
		{
	        _mappings=mappings;
	        _ontologyProvider=ontologyProvider;
		}
		#endregion

		#region Internal methods
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
					string whereClauseText=TransformWhereClause(queryModel,(WhereClause)clause,ref variableMap);
					if (!System.String.IsNullOrEmpty(whereClauseText))
					{
						whereClause+="\t"+whereClauseText+(whereClauseText.TrimEnd().EndsWith(".")?System.String.Empty:".")+Environment.NewLine;
					}
				}
			}

			string result=System.String.Format(ConstructTemplate,Environment.NewLine,whereClause);
#if DEBUG
			System.Diagnostics.Debug.WriteLine(result);
#endif
			return result;
		}

		internal string CreateSingleResultCommandText(QueryModel queryModel)
		{
			string result=CreateCommandText(queryModel);
			if (result.Length>0)
            {
				result=System.String.Format(SingleConstructTemplate,Environment.NewLine,result.Replace("\r",System.String.Empty).Replace("\n",System.Environment.NewLine+"\t\t").Replace("?s","?_s").Replace("CONSTRUCT { ?_s ?p ?o }","SELECT DISTINCT ?_s"));
            }

			return result;
		}
		#endregion

		#region Private methods
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
		#endregion

		#region Expression transformation methods
		private string TransformConstantExpression(ConstantExpression expression)
		{
			return System.String.Format(((expression.Type.IsValueType)||(expression.Type==typeof(string))?"\"{0}\"":"<{0}"),expression.Value.ToString());
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
				ExceptionHelper.ThrowInvalidOperationException(expression);
			}

			return result;
		}

		private string TransformMethodBinaryExpression(BinaryExpression expression)
		{
			string result=System.String.Empty;
			if (expression.Method.Name=="op_Equality")
			{
				MemberExpression member=(expression.Left as MemberExpression)??(expression.Right as MemberExpression);
				ConstantExpression constant=(expression.Left as ConstantExpression)??(expression.Right as ConstantExpression);
				UnaryExpression unary=(expression.Left as UnaryExpression)??(expression.Right as UnaryExpression);

				if ((member!=null)&&(member.Member.Name=="Id")&&(typeof(IEntity).IsAssignableFrom(member.Member.DeclaringType))&&(constant!=null))
			    {
					result=System.String.Format("?s ?p ?o.{0}FILTER(?s=<{1}>)",Environment.NewLine,constant.Value.ToString());
			    }			    
				else if ((unary!=null)&&(constant!=null))
                {
					result=System.String.Format("{0} {1}",TransformMethodUnaryExpression(unary),TransformConstantExpression(constant));
                }
			    else
			    {
					ExceptionHelper.ThrowInvalidOperationException(expression);
			    }
			}
			else
			{
				ExceptionHelper.ThrowInvalidOperationException(expression);
			}

			return result;
		}

		private string TransformTypeBinaryExpression(TypeBinaryExpression expression)
		{
			string result=System.String.Empty;
			if (expression.NodeType==ExpressionType.TypeIs)
			{
				if (!typeof(IEntity).IsAssignableFrom(expression.TypeOperand))
				{
					ExceptionHelper.ThrowInvalidCastException(typeof(IEntity),expression.TypeOperand);
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
						result=System.String.Format("?s a <{0}>.",mapping.Type.Uri.AbsoluteUri);
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

			return result;
		}
		#endregion
	}
}