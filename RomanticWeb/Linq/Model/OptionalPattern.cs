using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents a binary operator in the query.</summary>
    [QueryComponentNavigator(typeof(OptionalPatternNavigator))]
    public class OptionalPattern : QueryElement
    {
        #region Fields
        private IList<EntityConstrain> _patterns;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public OptionalPattern()
            : base()
        {
            _patterns = new List<EntityConstrain>();
        }
        #endregion

        #region Properties
        /// <summary>Gets an enumeration of optional patterns.</summary>
        public IList<EntityConstrain> Patterns { get { return _patterns; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this optional pattern.</summary>
        /// <returns>String representation of this optional pattern.</returns>
        public override string ToString()
        {
            return System.String.Format("OPTIONAL {{ {0} }}", System.String.Join(" ", _patterns));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(OptionalPattern)) &&
                (_patterns.SequenceEqual(((OptionalPattern)operand)._patterns));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = typeof(OptionalPattern).FullName.GetHashCode();
            foreach (EntityConstrain pattern in _patterns)
            {
                result ^= pattern.GetHashCode();
            }

            return result;
        }
        #endregion
    }
}