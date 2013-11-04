using Remotion.Linq;
using RomanticWeb.Mapping;

namespace RomanticWeb.Linq
{
    /// <summary>Converts LINQ query model to SPARQL abstraction.</summary>
    internal class QueryModelVisitor:QueryModelVisitorBase
    {
        private readonly IMappingsRepository _mappings;

        private SparqlQuery _sparqlQuery;

        public QueryModelVisitor(IMappingsRepository mappings)
        {
            _mappings=mappings;
        }

        public SparqlQuery SparqlQuery
        {
            get
            {
                return _sparqlQuery;
            }
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this, queryModel);
            VisitResultOperators(queryModel.ResultOperators, queryModel);
        }
    }
}