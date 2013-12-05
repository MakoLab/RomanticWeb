using System;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about entity constrain.</summary>
    [QueryComponentNavigator(typeof(EntityConstrainNavigator))]
    public class EntityConstrain:QueryElement
    {
        #region Fields
        private IExpression _predicate;
        private IExpression _value;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public EntityConstrain():base()
        {
        }

        /// <summary>Constructs a complete entity constrain.</summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="value">Object.</param>
        public EntityConstrain(IExpression predicate,IExpression value)
        {
            Predicate=predicate;
            Value=value;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets a predicate for this constrain.</summary>
        [AllowNull]
        public IExpression Predicate
        {
            get
            {
                return _predicate;
            }

            set
            {
                if (((_predicate=value)!=null)&&(_predicate is QueryComponent))
                {
                    ((QueryComponent)_predicate).OwnerQuery=OwnerQuery;
                }
            }
        }

        /// <summary>Gets or sets an object for this constrain.</summary>
        [AllowNull]
        public IExpression Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (((_value=value)!=null)&&(_value is QueryComponent))
                {
                    ((QueryComponent)_value).OwnerQuery=OwnerQuery;
                }
            }
        }

        /// <summary>Gets an owning query.</summary>
        internal override Query OwnerQuery
        {
            get
            {
                return base.OwnerQuery;
            }

            set
            {
                base.OwnerQuery=value;
                if ((_predicate!=null)&&(_predicate is QueryComponent))
                {
                    ((QueryComponent)_predicate).OwnerQuery=value;
                }

                if ((_value!=null)&&(_value is QueryComponent))
                {
                    ((QueryComponent)_value).OwnerQuery=value;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand is EntityConstrain)&&
                (_predicate!=null?_predicate.Equals(((EntityConstrain)operand)._predicate):Object.Equals(((EntityConstrain)operand)._predicate,null))&&
                (_value!=null?_value.Equals(((EntityConstrain)operand)._value):Object.Equals(((EntityConstrain)operand)._value,null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(EntityConstrain).FullName.GetHashCode()^(_predicate!=null?_predicate.GetHashCode():0)^(_value!=null?_value.GetHashCode():0);
        }

        /// <summary>Creates a string representation of this entity constrain.</summary>
        /// <returns>String representation of this entity constrain.</returns>
        public override string ToString()
        {
            return System.String.Format(
                "?s {0} {1} .",
                (_predicate!=null?_predicate.ToString():System.String.Empty),
                (_value!=null?_value.ToString():System.String.Empty));
        }
        #endregion
    }
}