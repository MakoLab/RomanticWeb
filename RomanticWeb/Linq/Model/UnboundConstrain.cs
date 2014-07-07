using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides details about entity constrain.</summary>
    [QueryComponentNavigator(typeof(UnboundConstrainNavigator))]
    public class UnboundConstrain : EntityConstrain, ISelectableQueryComponent
    {
        #region Fields
        private IExpression _subject;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public UnboundConstrain()
            : base()
        {
        }

        /// <summary>Constructs a complete unbound constrain.</summary>
        /// <param name="subject">Subject.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="value">Object.</param>
        /// <param name="targetExpression">Target expression that was source of this constrain.</param>
        public UnboundConstrain(IExpression subject, IExpression predicate, IExpression value, Expression targetExpression)
            : base(predicate, value, targetExpression)
        {
            Subject = subject;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets a subject for this constrain.</summary>
        [AllowNull]
        public IExpression Subject
        {
            get
            {
                return _subject;
            }

            set
            {
                if (((_subject = value) != null) && (_subject is QueryComponent))
                {
                    ((QueryComponent)_subject).OwnerQuery = OwnerQuery;
                }
            }
        }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> ISelectableQueryComponent.Expressions
        {
            get
            {
                List<IExpression> result = new List<IExpression>();
                if (_subject != null)
                {
                    result.Add(_subject);
                }

                if (Predicate != null)
                {
                    result.Add(Predicate);
                }

                if (Value != null)
                {
                    result.Add(Value);
                }

                return result.AsReadOnly();
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
                base.OwnerQuery = value;
                if ((_subject != null) && (_subject is QueryComponent))
                {
                    ((QueryComponent)_subject).OwnerQuery = value;
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
            return (!Object.Equals(operand, null)) && (operand is UnboundConstrain) &&
                (_subject != null ? _subject.Equals(((UnboundConstrain)operand)._subject) : Object.Equals(((UnboundConstrain)operand)._subject, null)) &&
                (Predicate != null ? Predicate.Equals(((UnboundConstrain)operand).Predicate) : Object.Equals(((UnboundConstrain)operand).Predicate, null)) &&
                (Value != null ? Value.Equals(((UnboundConstrain)operand).Value) : Object.Equals(((UnboundConstrain)operand).Value, null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(UnboundConstrain).FullName.GetHashCode() ^ (_subject != null ? _subject.GetHashCode() : 0) ^ (Predicate != null ? Predicate.GetHashCode() : 0) ^ (Value != null ? Value.GetHashCode() : 0);
        }

        /// <summary>Creates a string representation of this entity constrain.</summary>
        /// <returns>String representation of this entity constrain.</returns>
        public override string ToString()
        {
            return System.String.Format(
                "{0} {1} {2} .",
                (_subject != null ? _subject.ToString() : System.String.Empty),
                (Predicate != null ? Predicate.ToString() : System.String.Empty),
                (Value != null ? Value.ToString() : System.String.Empty));
        }
        #endregion
    }
}