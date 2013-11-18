using System;
using NullGuard;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides an abstract for query element.</summary>
    public abstract class QueryComponent:IQueryComponent
    {
        private Query _ownerQuery;

        /// <summary>Gets an owning query.</summary>
        public virtual Query OwnerQuery { [return: AllowNull] get { return _ownerQuery; } internal set { _ownerQuery=value; } }
    }
}