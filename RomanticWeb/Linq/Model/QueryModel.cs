using System;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents a query model.</summary>
    public class QueryModel
    {
        #region Fields
        private Query _query;
        private Uri _metaGraphUri;
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
        public Query Query
        {
            get
            {
                if (_metaGraphUri==null)
                {
                    throw new InvalidOperationException("Cannot browse a query without meta-graph URI.");
                }

                return _query;
            }
        }

        /// <summary>Sets a meta-graph URI.</summary>
        public Uri MetaGraphUri { get { return _metaGraphUri; } set { _metaGraphUri=value; } }
        #endregion
    }
}