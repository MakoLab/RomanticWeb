using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	/// <summary>Parses the query model into a SPARQL 1.1 compatile query.</summary>
	internal class EntityQueryModelVisitor:QueryModelVisitorBase
	{
		#region Fields
		private static readonly Regex TypedLiteralRegularExpression=new Regex("\"[^\"]*\"(?<typeName>\\^\\^xsd\\:.+)$");
		private List<QuerySourceReferenceExpression> _expressionSources;
		private StringBuilder _commandText;
		private IOntologyProvider _ontologyProvider;
		private IMappingsRepository _mappingsRepository;
		private EntityQueryVisitor _visitor;
		private bool _isSubQuery;
		private int _whereClauseEndIndex=-1;
		#endregion

		#region Constructors
		private EntityQueryModelVisitor(IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository):this(ontologyProvider,mappingsRepository,null)
		{
		}

		private EntityQueryModelVisitor(IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository,IEnumerable<QuerySourceReferenceExpression> expressionSources):base()
		{
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
			_expressionSources=new List<QuerySourceReferenceExpression>();
			_commandText=new StringBuilder();
			if (_isSubQuery=(expressionSources!=null))
			{
				_expressionSources.AddRange(expressionSources);
			}

			_visitor=new EntityQueryVisitor(_commandText,_expressionSources,_ontologyProvider,_mappingsRepository);
		}
		#endregion

		#region Public methods
		/// <summary>Visits a query model.</summary>
		/// <param name="queryModel">Query model to be visited.</param>
		public override void VisitQueryModel(QueryModel queryModel)
		{
			queryModel.SelectClause.Accept(this,queryModel);
		}

		/// <summary>Visits a select clause.</summary>
		/// <param name="selectClause">Select clause to be visited.</param>
		/// <param name="queryModel">Query model containing given select clause.</param>
		public override void VisitSelectClause(SelectClause selectClause,QueryModel queryModel)
		{
			QuerySourceReferenceExpression querySource=(QuerySourceReferenceExpression)selectClause.Selector;
			VisitExpression(querySource);
			_commandText.AppendFormat("SELECT ?s{0} ?p{0} ?o{0} ",_expressionSources.IndexOf(querySource));
			if ((queryModel.BodyClauses.Any(clause => clause is WhereClause))||(queryModel.ResultOperators.Count>0))
			{
				_commandText.AppendFormat("WHERE {{ ?s{0} ?p{0} ?o{0}. ",_expressionSources.IndexOf(querySource));
				_whereClauseEndIndex=_commandText.Length;
			}

			queryModel.MainFromClause.Accept(this,queryModel);
			VisitBodyClauses(queryModel.BodyClauses,queryModel);
			VisitResultOperators(queryModel.ResultOperators,queryModel);
			if (_whereClauseEndIndex!=-1)
			{
				_commandText.Append(" }");
			}

			if ((!_isSubQuery)&&(_commandText.Length>0))
			{
				_commandText.Insert(0,"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX owl: <http://www.w3.org/2002/07/owl#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>"+Environment.NewLine);
			}
		}

		/// <summary>Visits a main from clause.</summary>
		/// <param name="fromClause">Main from clause to be visited.</param>
		/// <param name="queryModel">Query model containing given from clause.</param>
		public override void VisitMainFromClause(MainFromClause fromClause,QueryModel queryModel)
		{
			if (_isSubQuery)
			{
				VisitExpression(fromClause.FromExpression);
			}
			else if ((fromClause.FromExpression.Type.GetGenericArguments().Length>0)&&(fromClause.FromExpression.Type.GetGenericArguments()[0]!=typeof(IEntity)))
			{
				MethodInfo mappingForMethodInfo=_mappingsRepository.GetType().GetMethod("MappingFor").MakeGenericMethod(new Type[] { fromClause.FromExpression.Type.GetGenericArguments()[0] });
				IEntityMapping entityMapping=(IEntityMapping)mappingForMethodInfo.Invoke(_mappingsRepository,null);
				if ((entityMapping!=null)&&(entityMapping.Class!=null))
				{
					Ontology ontology=
						_ontologyProvider.Ontologies.Where(
							item => entityMapping.Class.Uri.AbsoluteUri.StartsWith(item.BaseUri.AbsoluteUri)).FirstOrDefault();
					if (ontology!=null)
					{
						if (_whereClauseEndIndex==-1)
						{
							_commandText.AppendFormat("WHERE {{ ?s{0} ?p{0} ?o{0}. ",_expressionSources.IndexOf((QuerySourceReferenceExpression)queryModel.SelectClause.Selector));
							_whereClauseEndIndex=_commandText.Length;
						}

						_commandText.AppendFormat("?s0 a <{0}>. ",entityMapping.Class.Uri.AbsoluteUri);
					}
				}
			}

			base.VisitMainFromClause(fromClause,queryModel);
		}

		/// <summary>Visits a where clause.</summary>
		/// <param name="whereClause">Where clause to be visited.</param>
		/// <param name="queryModel">Query model containing given from clause.</param>
		/// <param name="index">Index of the where clause in the query model.</param>
		public override void VisitWhereClause(WhereClause whereClause,QueryModel queryModel,int index)
		{
			VisitExpression(whereClause.Predicate);
			base.VisitWhereClause(whereClause,queryModel,index);
		}

		/// <summary>Visits a result operator.</summary>
		/// <param name="resultOperator">Result operator to be visided.</param>
		/// <param name="queryModel">Query model containing given result operator.</param>
		/// <param name="index">Index of the result operator in the query model.</param>
		public override void VisitResultOperator(ResultOperatorBase resultOperator,QueryModel queryModel,int index)
		{
			switch (resultOperator.GetType().FullName)
			{
				case "Remotion.Linq.Clauses.ResultOperators.AnyResultOperator":
					_commandText.Insert(_whereClauseEndIndex,"FILTER EXISTS { ").Append(" }");
					break;
			}

			base.VisitResultOperator(resultOperator,queryModel,index);
		}
		#endregion

		#region Non-public methods
		/// <summary>Creates a SPARQL command text for given query model.</summary>
		/// <param name="queryModel">Query model to be parsed.</param>
		/// <param name="ontologyProvider">Ontology provider containing data scheme.</param>
		/// <param name="mappingsRepository">Mappings repository used to resolve strongly typed properties and types.</param>
		/// <returns>String containing a SPARQL query or an empty string.</returns>
		internal static string CreateCommandText(QueryModel queryModel,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository)
		{
			return CreateCommandText(queryModel,null,ontologyProvider,mappingsRepository);
		}

		/// <summary>Creates a SPARQL command text for given query model.</summary>
		/// <param name="queryModel">Query model to be parsed.</param>
		/// <param name="querySources">Enumeration of query sources already acknowledged when parsing parent queries.</param>
		/// <param name="ontologyProvider">Ontology provider containing data scheme.</param>
		/// <param name="mappingsRepository">Mappings repository used to resolve strongly typed properties and types.</param>
		/// <returns>String containing a SPARQL query or an empty string.</returns>
		internal static string CreateCommandText(QueryModel queryModel,IEnumerable<QuerySourceReferenceExpression> querySources,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository)
		{
			EntityQueryModelVisitor queryModelVisitor=new EntityQueryModelVisitor(ontologyProvider,mappingsRepository,querySources);
			queryModelVisitor.VisitQueryModel(queryModel);
			return queryModelVisitor._commandText.ToString();
		}

		/// <summary>Visits body clauses.</summary>
		/// <param name="bodyClauses">Body clause to be visited.</param>
		/// <param name="queryModel">Query model containing given body clause.</param>
		protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses,QueryModel queryModel)
		{
			foreach (var indexValuePair in bodyClauses.AsChangeResistantEnumerableWithIndex())
			{
				string before=_commandText.ToString();
				indexValuePair.Value.Accept(this,queryModel,indexValuePair.Index);
				string graphPattern=_commandText.ToString().Substring(before.Length);
				Match typedLiteralMatch=TypedLiteralRegularExpression.Match(graphPattern);
				if ((typedLiteralMatch!=null)&&(typedLiteralMatch.Groups["typeName"]!=null)&&(typedLiteralMatch.Groups["typeName"].Length>0))
				{
					graphPattern=graphPattern.Remove(typedLiteralMatch.Groups["typeName"].Index,typedLiteralMatch.Groups["typeName"].Length);
					_commandText.Insert(before.Length,graphPattern+". OPTIONAL { ").Append(" }");
				}
			}
		}

		private void VisitExpression(Expression expression)
		{
			_visitor.VisitExpression(expression);
		}
		#endregion
	}
}