using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Expresses a literal in the query.</summary>
    [QueryComponentNavigator(typeof(ListNavigator))]
    public class List : QueryComponent, IExpression
    {
        #region Fields
        private IList<IExpression> _values;
        #endregion

        #region Constructors
        /// <summary>Base parameterles constructor.</summary>
        public List()
            : base()
        {
            _values = new List<IExpression>();
        }
        #endregion

        #region Properties
        /// <summary>Gets a list of values.</summary>
        public IList<IExpression> Values { get { return _values; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this literal.</summary>
        /// <returns>String representation of this literal.</returns>
        public override string ToString()
        {
            return System.String.Format("({0})", System.String.Join(" ", _values));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(List)) && (_values.SequenceEqual(((List)operand)._values));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = typeof(List).FullName.GetHashCode();
            foreach (IExpression value in _values)
            {
                result ^= value.GetHashCode();
            }

            return result;
        }
        #endregion
    }
}