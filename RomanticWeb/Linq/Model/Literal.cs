using System;
using System.Globalization;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Expresses a literal in the query.</summary>
    public class Literal:QueryComponent,IExpression
    {
        #region Fields
        private object _value;
        #endregion

        #region Constructors
        /// <summary>Base constructor with value passed.</summary>
        /// <param name="value">Value of this literal.</param>
        public Literal(object value):base()
        {
            _value=value;
        }
        #endregion

        #region Properties
        /// <summary>Gets a value of this literal.</summary>
        public object Value { get { return _value; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this literal.</summary>
        /// <returns>String representation of this literal.</returns>
        public override string ToString()
        {
            string valueString=System.String.Empty;
            if (_value!=null)
            {
                switch (_value.GetType().FullName)
                {
                    default:
                    case "System.Byte":
                    case "System.SByte":
                    case "System.Int16":
                    case "System.UInt16":
                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int64":
                    case "System.UInt64":
                        valueString=_value.ToString();
                        break;
                    case "System.Char":
                        valueString=System.String.Format("'{0}'",_value);
                        break;
                    case "System.TimeSpan":
                    case "System.String":
                        valueString=System.String.Format("\"{0}\"",_value);
                        break;
                    case "System.Single":
                    case "System.Double":
                    case "System.Decimal":
                    case "System.DateTime":
                        valueString=System.String.Format(CultureInfo.InvariantCulture,"{0}",_value);
                        break;
                    case "System.Uri":
                        valueString=System.String.Format("<{0}>",_value);
                        break;
                }
            }

            return valueString;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals(object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(Literal))&&(_value!=null?_value.Equals(((Literal)operand)._value):false);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(Literal).FullName.GetHashCode()^(_value!=null?_value.GetHashCode():0);
        }
        #endregion
    }
}