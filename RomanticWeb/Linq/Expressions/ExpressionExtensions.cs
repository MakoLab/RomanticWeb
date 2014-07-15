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
                if ((memberA.Expression == memberB.Expression) && (memberA.Member == memberB.Member) && (memberA.NodeType == memberB.NodeType)) { return true; }
            }

            return false;
        }
    }
}