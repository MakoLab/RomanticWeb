using System;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about filter.</summary>
    [QueryComponentNavigator(typeof(FilterNavigator))]
    public class Filter : QueryElement
    {
        #region Fields
        private IExpression _expression;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public Filter()
            : base()
        {
        }

        /// <summary>Default constructor with filter expresion passed.</summary>
        /// <param name="expression">Filter expression.</param>
        public Filter(IExpression expression)
            : base()
        {
            Expression = expression;
        }
        #endregion

        #region Properties
        /// <summary>Gets a filter expression.</summary>
        [AllowNull]
        public IExpression Expression { get { return _expression; } set { _expression = value; } }

        /// <summary>Gets an owning query.</summary>
        internal override Query OwnerQuery
        {
            get
            {
                return base.OwnerQuery;
            }

            set
            {
                base.OwnerQuery = value;
                if ((_expression != null) && (_expression is QueryComponent))
                {
                    ((QueryComponent)_expression).OwnerQuery = OwnerQuery;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this filter.</summary>
        /// <returns>String representation of this filter.</returns>
        public override string ToString()
        {
            return System.String.Format("FILTER ({0})", (_expression != null ? _expression.ToString() : System.String.Empty));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(Filter)) &&
                (_expression != null ? _expression.Equals(((Filter)operand)._expression) : Object.Equals(((Filter)operand)._expression, null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(Filter).FullName.GetHashCode() ^ (_expression != null ? _expression.GetHashCode() : 0);
        }
        #endregion
    }
}