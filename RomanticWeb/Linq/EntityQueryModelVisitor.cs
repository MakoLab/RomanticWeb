using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Collections;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	internal class EntityQueryModelVisitor:QueryModelVisitorBase
	{
		private static readonly Regex TypedLiteralRegularExpression=new Regex("\"[^\"]*\"(?<typeName>\\^\\^xsd\\:.+)$");
		private List<QuerySourceReferenceExpression> _expressionSources;
		private StringBuilder _commandText;
		private IOntologyProvider _ontologyProvider;
		private IMappingsRepository _mappingsRepository;
		private EntityQueryVisitor _visitor;
		private bool _isSubQuery;

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

		public override void VisitQueryModel(QueryModel queryModel)
		{
			queryModel.SelectClause.Accept(this,queryModel);
		}

		public override void VisitSelectClause(SelectClause selectClause,QueryModel queryModel)
		{
			QuerySourceReferenceExpression querySource=(QuerySourceReferenceExpression)selectClause.Selector;
			VisitExpression(querySource);
			_commandText.AppendFormat("SELECT ?s{0} ?p{0} ?o{0} ",_expressionSources.IndexOf(querySource));
			bool isWhereClause=queryModel.BodyClauses.Any(clause => clause is WhereClause);
			if (isWhereClause)
			{
				_commandText.AppendFormat("WHERE {{ ?s{0} ?p{0} ?o{0}. ",_expressionSources.IndexOf(querySource));
			}

			queryModel.MainFromClause.Accept(this,queryModel);
			VisitBodyClauses(queryModel.BodyClauses,queryModel);
			VisitResultOperators(queryModel.ResultOperators,queryModel);
			if (isWhereClause)
			{
				_commandText.AppendFormat(" }}");
			}

			if (!_isSubQuery)
			{
				_commandText.Insert(0,"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX owl: <http://www.w3.org/2002/07/owl#>"+Environment.NewLine);
				_commandText.Insert(0,"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>"+Environment.NewLine);
			}
		}

		public override void VisitMainFromClause(MainFromClause fromClause,QueryModel queryModel)
		{
			if (_isSubQuery)
			{
				VisitExpression(fromClause.FromExpression);
			}

			base.VisitMainFromClause(fromClause,queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause,QueryModel queryModel,int index)
		{
			VisitExpression(whereClause.Predicate);
			base.VisitWhereClause(whereClause,queryModel,index);
		}

		internal static string CreateCommandText(QueryModel queryModel,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository)
		{
			return CreateCommandText(queryModel,null,ontologyProvider,mappingsRepository);
		}

		internal static string CreateCommandText(QueryModel queryModel,IEnumerable<QuerySourceReferenceExpression> querySources,IOntologyProvider ontologyProvider,IMappingsRepository mappingsRepository)
		{
			EntityQueryModelVisitor queryModelVisitor=new EntityQueryModelVisitor(ontologyProvider,mappingsRepository,querySources);
			queryModelVisitor.VisitQueryModel(queryModel);
			return queryModelVisitor._commandText.ToString();
		}

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
	}
}