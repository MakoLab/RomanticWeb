using RomanticWeb.Linq.Model;

namespace RomanticWeb.Linq
{
    /// <summary>Provides a base interface for LINQ to SPARQL query optimization routines.</summary>
    public interface IQueryOptimizer
    {
        /// <summary>Optimizes given query</summary>
        /// <param name="query">Query to be optimized.</param>
        /// <returns>Optimized query.</returns>
        Query Optimize(Query query);
    }
}