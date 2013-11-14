using System;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides an abstraction for operators.</summary>
    public abstract class Operator:Call
    {
        #region Constructors
        /// <summary>Default constructor with operator name.</summary>
        /// <param name="operatorName">Operator name.</param>
        public Operator(MethodNames operatorName):base(operatorName)
        {
            if ((operatorName&Call.Operator)!=Call.Operator)
            {
                throw new ArgumentOutOfRangeException(System.String.Format("Invalid operator '{0}'.",operatorName));
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this operator.</summary>
        /// <returns>String representation of this operator.</returns>
        public override string ToString()
        {
            return Member.ToString();
        }
        #endregion
    }
}