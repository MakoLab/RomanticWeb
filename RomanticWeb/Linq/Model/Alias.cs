using System;
using System.Collections.Generic;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Expresses an alias in the query.</summary>
    [QueryComponentNavigator(typeof(AliasNavigator))]
    public class Alias:QueryComponent,IExpression,ISelectableQueryComponent
    {
        #region Fields
        private IQueryComponent _component;
        private Identifier _name;
        #endregion

        #region Constructors
        /// <summary>Base constructor with component and alias passed.</summary>
        /// <param name="component">Component to be aliased.</param>
        /// <param name="name">Alias.</param>
        public Alias(IQueryComponent component,Identifier name):base()
        {
            _component=component;
            _name=name;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets a component aliased.</summary>
        public IQueryComponent Component { get { return _component; } set { _component=value; } }

        /// <summary>Gets or sets name of the alias.</summary>
        public Identifier Name { get { return _name; } set { _name=value; } }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        public IEnumerable<IExpression> Expressions { get { return new IExpression[] { this }; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this literal.</summary>
        /// <returns>String representation of this literal.</returns>
        public override string ToString()
        {
            return (_component!=null?_component.ToString():System.String.Empty)+(_name!=null?" AS "+_name.ToString():System.String.Empty);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(Alias))&&
                (_component!=null?_component.Equals(((Alias)operand)._component):Object.Equals(((Alias)operand)._component,null))&&
                (_name!=null?_name.Equals(((Alias)operand)._name):Object.Equals(((Alias)operand)._name,null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(Alias).FullName.GetHashCode()^(_component!=null?_component.GetHashCode():0)^(_name!=null?_name.GetHashCode():0);
        }
        #endregion
    }
}