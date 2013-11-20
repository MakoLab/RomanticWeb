using System;
using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Serves as a base navigator class.</summary>
    internal abstract class QueryComponentNavigatorBase:IQueryComponentNavigator
    {
        #region Fields
        private IQueryComponent _component;
        #endregion

        #region Constructors
        /// <summary>Creates a component navigator.</summary>
        /// <param name="component">Navigated component.</param>
        internal QueryComponentNavigatorBase(IQueryComponent component)
        {
            _component=component;
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        IQueryComponent IQueryComponentNavigator.NavigatedComponent { get { return _component; } }
        #endregion

        #region Public methods
        /// <summary>Checks for equality of two query component operators.</summary>
        /// <param name="operandA">Left operand.</param>
        /// <param name="operandB">Right operand.</param>
        /// <returns><b>true</b> if both operands has the navigated component equals, otherwise <b>false</b>.</returns>
        public static bool operator==(QueryComponentNavigatorBase operandA,QueryComponentNavigatorBase operandB)
        {
            return ((!Object.Equals(operandA,null))&&(!Object.Equals(operandB,null))&&(operandA._component==operandB._component))||(Object.Equals(operandA,null)&&(Object.Equals(operandB,null)));
        }

        /// <summary>Checks for inequality of two query component operators.</summary>
        /// <param name="operandA">Left operand.</param>
        /// <param name="operandB">Right operand.</param>
        /// <returns><b>true</b> if both operands has the navigated component different, otherwise <b>false</b>.</returns>
        public static bool operator!=(QueryComponentNavigatorBase operandA,QueryComponentNavigatorBase operandB)
        {
            return ((Object.Equals(operandA,null))&&(!Object.Equals(operandB,null)))||
                ((!Object.Equals(operandA,null))&&(Object.Equals(operandB,null)))||
                ((!Object.Equals(operandA,null))&&(!Object.Equals(operandB,null))&&(operandA._component!=operandB._component));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// true  if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object operand)
        {
            return (!Object.Equals(operand,null))&&(operand is IQueryComponentNavigator)&&(_component.Equals(((IQueryComponentNavigator)operand).NavigatedComponent));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return _component.GetHashCode();
        }

        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public abstract bool CanAddComponent(IQueryComponent component);

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public abstract bool ContainsComponent(IQueryComponent component);

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public abstract void AddComponent(IQueryComponent component);

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public abstract void ReplaceComponent(IQueryComponent component,IQueryComponent replacement);

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public abstract IEnumerable<IQueryComponent> GetComponents();
        #endregion
    }
}