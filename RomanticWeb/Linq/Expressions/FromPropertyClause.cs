using System;
using System.Reflection;

namespace RomanticWeb.Linq.Expressions
{
    /// <summary>Provides a facade for property query source.</summary>
    internal class FromPropertyClause:Remotion.Linq.Clauses.FromClauseBase
    {
        /// <summary>Creates an instance from <see cref="FromPropertyClause" />.</summary>
        /// <param name="itemName">Item name.</param>
        /// <param name="itemType">Item type.</param>
        /// <param name="fromExpression">Property expression.</param>
        internal FromPropertyClause(string itemName,Type itemType,System.Linq.Expressions.MemberExpression fromExpression):base(itemName,itemType,fromExpression)
        {
            if (!(fromExpression.Member is PropertyInfo))
            {
                throw new ArgumentOutOfRangeException("fromExpression");
            }
        }
    }
}