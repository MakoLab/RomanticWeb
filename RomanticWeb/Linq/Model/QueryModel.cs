using System;
using NullGuard;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents a query model.</summary>
    public class QueryModel
    {
        #region Fields
        private Query _query;
        #endregion

        #region Constructors
        /// <summary>Creates a model from query and additional details.</summary>
        /// <param name="query">Query.</param>
        internal QueryModel(Query query)
        {
            _query=query;
        }

        /// <summary>Creates a model from query and additional details.</summary>
        /// <param name="query">Query.</param>
        /// <param name="metaGraphUri">URI of the meta-graph.</param>
        internal QueryModel(Query query,Uri metaGraphUri)
        {
            _query=query;
            _metaGraphUri=metaGraphUri;
        }
        #endregion

        #region Properties
        /// <summary>Gets a query.</summary>
        public Query Query { get { return _query; } }
        #endregion
    }
}