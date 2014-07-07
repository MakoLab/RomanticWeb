using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Represents an entity type constrain.</summary>
    [QueryComponentNavigator(typeof(EntityTypeConstrainNavigator))]
    public class EntityTypeConstrain : EntityConstrain
    {
        #region Fields
        private static readonly Literal TypePredicate = new Literal(RomanticWeb.Vocabularies.Rdf.type);
        private IEnumerable<Literal> _inheritedTypes = new Literal[0];
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public EntityTypeConstrain()
            : base()
        {
        }

        /// <summary>Constructor with entity type Uri passed.</summary>
        /// <param name="type">Entity type</param>
        /// <param name="targetExpression">Target expression that was source of this constrain.</param>
        /// <param name="inheritedTypes">Optional inherited types of the entity.</param>
        public EntityTypeConstrain(Uri type, Expression targetExpression, params Uri[] inheritedTypes)
            : base(TypePredicate, new Literal(type), targetExpression)
        {
            _inheritedTypes = inheritedTypes.Select(item => new Literal(item));
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets a predicate for this constrain.</summary>
        public override IExpression Predicate
        {
            get { return TypePredicate; }
            set { }
        }

        /// <summary>Gets or sets an object for this constrain.</summary>
        [AllowNull]
        public override IExpression Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                if (value != null)
                {
                    if ((value is Literal) && (((Literal)value).Value is Uri))
                    {
                        base.Value = value;
                    }
                }
                else
                {
                    base.Value = null;
                }
            }
        }

        /// <summary>Gets an enumeration of inherited entity types.</summary>
        public IEnumerable<IExpression> InheritedTypes { get { return _inheritedTypes; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this entity type constrain.</summary>
        /// <returns>String representation of this entity type constrain.</returns>
        public override string ToString()
        {
            return System.String.Format("?s {0} {1}", TypePredicate, (Value != null ? Value.ToString() : "?o"));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(EntityTypeConstrain)) &&
                (Value != null ? Value.Equals(((EntityTypeConstrain)operand).Value) : ((EntityTypeConstrain)operand).Value == null);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(EntityTypeConstrain).FullName.GetHashCode() ^ (Value != null ? Value.GetHashCode() : 0);
        }
        #endregion
    }
}