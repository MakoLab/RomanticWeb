using System;
using System.Collections.Specialized;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents a binary operator in the query.</summary>
    [QueryComponentNavigator(typeof(BinaryOperatorNavigator))]
    public class BinaryOperator:UnaryOperator
    {
        #region Constructors
        /// <summary>Default constructor with operator name.</summary>
        /// <param name="operatorName">Operator name.</param>
        public BinaryOperator(MethodNames operatorName):base(operatorName)
        {
        }

        /// <summary>Constructor with operator name and both operands passed.</summary>
        /// <param name="operatorName">Operator name.</param>
        /// <param name="leftOperand">Left operand.</param>
        /// <param name="rightOperand">Right operand.</param>
        public BinaryOperator(MethodNames operatorName,IExpression leftOperand,IExpression rightOperand):base(operatorName,leftOperand)
        {
            RightOperand=rightOperand;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets left operand of this operator.</summary>
        [AllowNull]
        public IExpression LeftOperand { get { return Operand; } set { Operand=value; } }

        /// <summary>Gets or sets right operand of this operator.</summary>
        [AllowNull]
        public IExpression RightOperand
        {
            get
            {
                return (Arguments.Count>1?Arguments[1]:null);
            }

            set
            {
                if (Arguments.Count<2)
                {
                    if (Arguments.Count<1)
                    {
                        Arguments[0]=null;
                    }

                    if (value!=null)
                    {
                        Arguments.Add(value);
                    }
                }
                else
                {
                    if (value!=null)
                    {
                        Arguments[1]=value;
                    }
                    else
                    {
                        Arguments.RemoveAt(1);
                    }
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this binary operator.</summary>
        /// <returns>String representation of this binary operator.</returns>
        public override string ToString()
        {
            string operatorString=Member.ToString();
            switch (Member)
            {
                case MethodNames.Add:
                    operatorString="+";
                    break;
                case MethodNames.AddAndAssign:
                    operatorString="+=";
                    break;
                case MethodNames.Substract:
                    operatorString="-";
                    break;
                case MethodNames.SubstractAndAssign:
                    operatorString="-=";
                    break;
                case MethodNames.Multiply:
                    operatorString="*";
                    break;
                case MethodNames.MultiplyAndAssign:
                    operatorString="*=";
                    break;
                case MethodNames.Divide:
                    operatorString="/";
                    break;
                case MethodNames.DivideAndAssign:
                    operatorString="/=";
                    break;
                case MethodNames.Modulo:
                    operatorString="%";
                    break;
                case MethodNames.ModuloAndAssign:
                    operatorString="%=";
                    break;
                case MethodNames.Equal:
                    operatorString="==";
                    break;
                case MethodNames.NotEqual:
                    operatorString="!=";
                    break;
                case MethodNames.GreaterThan:
                    operatorString=">";
                    break;
                case MethodNames.GreaterThanOrEqual:
                    operatorString=">=";
                    break;
                case MethodNames.LessThan:
                    operatorString="<";
                    break;
                case MethodNames.LessThanOrEqual:
                    operatorString="<=";
                    break;
                case MethodNames.BitwiseAnd:
                    operatorString="&";
                    break;
                case MethodNames.BitwiseAndAndAssign:
                    operatorString="&=";
                    break;
                case MethodNames.BitwiseOr:
                    operatorString="|";
                    break;
                case MethodNames.BitwiseOrAndAssign:
                    operatorString="|=";
                    break;
                case MethodNames.BitwiseXor:
                    operatorString="^";
                    break;
                case MethodNames.BitwiseXorAndAssign:
                    operatorString="^=";
                    break;
                case MethodNames.BitwiseNot:
                    operatorString="~";
                    break;
                case MethodNames.BitwiseNotAndAssign:
                    operatorString="~=";
                    break;
                case MethodNames.And:
                    operatorString="&&";
                    break;
                case MethodNames.Or:
                    operatorString="||";
                    break;
                case MethodNames.Xor:
                    operatorString="^^";
                    break;
                case MethodNames.BitwiseShiftLeft:
                    operatorString="<<";
                    break;
                case MethodNames.BitwiseShiftLeftAndAssign:
                    operatorString="<<=";
                    break;
                case MethodNames.BitwiseShiftRight:
                    operatorString=">>";
                    break;
                case MethodNames.BitwiseShiftRightAndAssign:
                    operatorString=">>=";
                    break;
            }

            return System.String.Format(
                "{0}{1}{2}",
                (LeftOperand!=null?LeftOperand.ToString():System.String.Empty),
                operatorString,
                (RightOperand!=null?RightOperand.ToString():System.String.Empty));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(BinaryOperator))&&
                (LeftOperand!=null?LeftOperand.Equals(((BinaryOperator)operand).LeftOperand):Object.Equals(((BinaryOperator)operand).LeftOperand,null))&&
                (RightOperand!=null?RightOperand.Equals(((BinaryOperator)operand).RightOperand):Object.Equals(((BinaryOperator)operand).RightOperand,null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(BinaryOperator).FullName.GetHashCode()^(LeftOperand!=null?LeftOperand.GetHashCode():0)^(RightOperand!=null?RightOperand.GetHashCode():0);
        }
        #endregion

        #region Non-public methods
        /// <summary>Rised when arguments collection has changed.</summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Eventarguments.</param>
        protected override void OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (QueryComponent queryComponent in e.NewItems)
                        {
                            if (queryComponent!=null)
                            {
                                queryComponent.OwnerQuery=OwnerQuery;
                            }
                        }

                        if (Arguments.Count>2)
                        {
                            throw new InvalidOperationException("Cannot add more than two operands for binary operator.");
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}