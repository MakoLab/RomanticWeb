using System;
using FluentAssertions;
using FluentAssertions.Primitives;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Builder.Expressions;
using VDS.RDF.Query.Datasets;

namespace RomanticWeb.Tests.Helpers
{
    public class StoreAssertions : ReferenceTypeAssertions<ITripleStore, StoreAssertions>
    {
        private readonly ISparqlQueryProcessor _queryProcessor;

        public StoreAssertions(IInMemoryQueryableStore store)
        {
            _queryProcessor = new LeviathanQueryProcessor(new InMemoryQuadDataset(store, true));
        }

        protected override string Context
        {
            get
            {
                return "triple store";
            }
        }

        /// <summary>
        /// Checks that triple store contains triples by evaluating an ASK query over union graph
        /// </summary>
        /// <param name="buildPatterns">Action to set up triple patterns</param>
        public AndConstraint<StoreAssertions> MatchAsk(Action<ITriplePatternBuilder> buildPatterns, Func<ExpressionBuilder, BooleanExpression> filter = null)
        {
            var ask = QueryBuilder.Ask().Where(buildPatterns);

            if (filter != null)
            {
                ask.Filter(filter);
            }

            var sparqlQuery = ask.BuildQuery();
            var result = (SparqlResultSet)_queryProcessor.ProcessQuery(sparqlQuery);
            result.Result.Should().Be(true, "RDF data should match query '{0}'", sparqlQuery);

            return new AndConstraint<StoreAssertions>(this);
        }
    }
}