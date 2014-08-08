using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Linq.Expressions
{
    internal static class ExpressionExtensions
    {
        internal static bool EqualsTo(this Expression operandA, Expression operandB)
        {
            if ((Object.Equals(operandA, null)) && (Object.Equals(operandB, null))) { return true; }
            if (((Object.Equals(operandA, null)) && (!Object.Equals(operandB, null))) || ((!Object.Equals(operandA, null)) && (Object.Equals(operandB, null)))) { return false; }
            if (Object.ReferenceEquals(operandA, operandB)) { return true; }
            if (operandA.GetType() != operandB.GetType()) { return false; }
            if (operandA is MemberExpression)
            {
                MemberExpression memberA = (MemberExpression)operandA;
                MemberExpression memberB = (MemberExpression)operandB;
                if ((memberA.Expression.EqualsTo(memberB.Expression)) && (memberA.Member == memberB.Member) && (memberA.NodeType == memberB.NodeType)) { return true; }
            }
            else if (operandA is Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)
            {
                Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression sourceA = (Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)operandA;
                Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression sourceB = (Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)operandB;
                if (sourceA.ReferencedQuerySource == sourceB.ReferencedQuerySource) { return true; }
            }

            return false;
        }

        internal static bool EqualsTo(this Remotion.Linq.Clauses.FromClauseBase operandA, Remotion.Linq.Clauses.FromClauseBase operandB)
        {
            if ((Object.Equals(operandA, null)) && (Object.Equals(operandB, null))) { return true; }
            if (((Object.Equals(operandA, null)) && (!Object.Equals(operandB, null))) || ((!Object.Equals(operandA, null)) && (Object.Equals(operandB, null)))) { return false; }
            if (Object.ReferenceEquals(operandA, operandB)) { return true; }
            if (operandA.GetType() != operandB.GetType()) { return false; }
            if ((operandA.ItemName == operandB.ItemName) && (operandA.ItemType == operandB.ItemType) && (operandA.FromExpression.EqualsTo(operandB.FromExpression))) { return true; }
            return false;
        }
    }
}