using System;
using RomanticWeb.Linq.Model;

namespace RomanticWeb.Linq
{
    /// <summary>Optimizes SPARQL 1.1 query.</summary>
    public class GenericQueryOptimizer:IQueryOptimizer
    {
        /// <inheritdoc />
        public Query Optimize(Query query)
        {
            return query;
        }
    }
}