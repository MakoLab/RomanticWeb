using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RomanticWeb.Linq.Expressions
{
    /// <summary>Provides basic details about entity identifier access.</summary>
    public class EntityIdentifierExpression:System.Linq.Expressions.Expression
    {
        #region Fields
        private System.Linq.Expressions.MemberExpression _expression;
        private Remotion.Linq.Clauses.FromClauseBase _target;
        #endregion

        #region Constructors
        /// <summary>Default constructor with base <see cref="System.Linq.Expressions.MemberExpression" /> and <see cref="Remotion.Linq.Clauses.FromClauseBase" />.</summary>
        /// <param name="expression">Base member expression.</param>
        /// <param name="target">Target of the invocation.</param>
        internal EntityIdentifierExpression(System.Linq.Expressions.MemberExpression expression,Remotion.Linq.Clauses.FromClauseBase target):base()
        {
            if (!(expression.Member is PropertyInfo))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            _expression=expression;
            _target=target;
        }
        #endregion

        #region Properties
        /// <summary>Base member expression.</summary>
        public System.Linq.Expressions.MemberExpression Expression { get { return _expression; } }

        /// <summary>EntityProperty accessed.</summary>
        public PropertyInfo EntityProperty { get { return (PropertyInfo)_expression.Member; } }

        /// <summary>Determines if the expression can be reduced.</summary>
        public override bool CanReduce { get { return _expression.CanReduce; } }

        /// <summary>Gets the node type.</summary>
        public override ExpressionType NodeType { get { return _expression.NodeType; } }

        /// <summary>Gets the type returned by given member.</summary>
        public override Type Type { get { return _expression.Type; } }

        /// <summary>Gets the target of the invocation.</summary>
        public Remotion.Linq.Clauses.FromClauseBase Target { get { return _target; } }
        #endregion
    }
}