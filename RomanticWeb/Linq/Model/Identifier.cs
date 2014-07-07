using System;
using System.Collections.Generic;
using NullGuard;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Expresses a literal in the query.</summary>
    public class Identifier : QueryComponent, IExpression, ISelectableQueryComponent
    {
        #region Fields
        /// <summary>Gets a meta-identiefier that refers to current context's subject.</summary>
        public static readonly Identifier Current = new Identifier("s", typeof(object));
        private string _name;
        private Type _nativeType;
        #endregion

        #region Constructors
        /// <summary>Base constructor with name passed.</summary>
        /// <param name="name">Name of this identifier.</param>
        public Identifier(string name)
            : this(name, typeof(object))
        {
        }

        /// <summary>Base constructor with name passed.</summary>
        /// <param name="name">Name of this identifier.</param>
        /// <param name="nativeType">Native type of the identifier.</param>
        public Identifier(string name, Type nativeType)
            : base()
        {
            _name = name;
            _nativeType = nativeType;
        }
        #endregion

        #region Properties
        /// <summary>Gets a name of this identifier.</summary>
        public string Name { get { return _name; } }

        /// <summary>Gets a native type represented by given identifier.</summary>
        public Type NativeType { get { return _nativeType; } }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> ISelectableQueryComponent.Expressions { get { return new IExpression[] { this }; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this identifier.</summary>
        /// <returns>String representation of this identifier.</returns>
        public override string ToString()
        {
            return System.String.Format("?{0}", _name);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand, null)) && (operand.GetType() == typeof(Identifier)) && (_name.Equals(((Identifier)operand)._name));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(Identifier).FullName.GetHashCode() ^ _name.GetHashCode();
        }
        #endregion
    }
}