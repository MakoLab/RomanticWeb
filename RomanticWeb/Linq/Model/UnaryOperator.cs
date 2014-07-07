using System;
using System.Collections.Specialized;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents an unary operator.</summary>
    [QueryComponentNavigator(typeof(UnaryOperatorNavigator))]
    public class UnaryOperator : Operator
    {
        #region Constructors
        /// <summary>Default constructor with operator name.</summary>
        /// <param name="operatorName">Operator name.</param>
        public UnaryOperator(MethodNames operatorName)
            : base(operatorName)
        {
        }

        /// <summary>Constructor with operator name and both operands passed.</summary>
        /// <param name="operatorName">Operator name.</param>
        /// <param name="operand">Operand.</param>
        public UnaryOperator(MethodNames operatorName, IExpression operand)
            : base(operatorName)
        {
            Operand = operand;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets an operand of this operator.</summary>
        [AllowNull]
        public IExpression Operand
        {
            get
            {
                return (Arguments.Count > 0 ? Arguments[0] : null);
            }

            set
            {
                if (Arguments.Count < 1)
                {
                    if (value != null)
                    {
                        Arguments.Add(value);
                    }
                }
                else
                {
                    if (value != null)
                    {
                        Arguments[0] = value;
                    }
                    else
                    {
                        if (Arguments.Count > 1)
                        {
                            Arguments[0] = null;
                        }
                        else
                        {
                            Arguments.RemoveAt(0);
                        }
                    }
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this unary operator.</summary>
        /// <returns>String representation of this unary operator.</returns>
        public override string ToString()
        {
            string operatorString = Member.ToString();
            switch (Member)
            {
                case MethodNames.Not:
                    operatorString = "!";
                    break;
            }

            return System.String.Format("{0}{1}", operatorString, (Operand != null ? Operand.ToString() : System.String.Empty));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(BinaryOperator)) &&
                (Operand != null ? Operand.Equals(((UnaryOperator)operand).Operand) : Object.Equals(((BinaryOperator)operand).Operand, null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(UnaryOperator).FullName.GetHashCode() ^ (Operand != null ? Operand.GetHashCode() : 0);
        }
        #endregion

        #region Non-public methods
        /// <summary>Rised when arguments collection has changed.</summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Eventarguments.</param>
        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(sender, e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (Arguments.Count > 1)
                        {
                            throw new InvalidOperationException("Cannot add more than one operand for unary operator.");
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}