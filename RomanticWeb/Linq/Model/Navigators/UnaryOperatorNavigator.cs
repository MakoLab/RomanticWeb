using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates unary operator.</summary>
    internal class UnaryOperatorNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated unary operator.</summary>
        /// <param name="unaryOperator">Nagivated unary operator.</param>
        internal UnaryOperatorNavigator(UnaryOperator unaryOperator)
            : base(unaryOperator)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public UnaryOperator NavigatedComponent { get { return (UnaryOperator)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is IExpression) && (NavigatedComponent.Operand == null);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (NavigatedComponent.Operand == component);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if ((component is IExpression) && (NavigatedComponent.Operand == null))
            {
                NavigatedComponent.Operand = (IExpression)component;
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is IExpression) && (replacement is IExpression))
            {
                if (NavigatedComponent.Operand == (IExpression)component)
                {
                    NavigatedComponent.Operand = (IExpression)replacement;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            return (NavigatedComponent.Operand != null ? new IQueryComponent[] { NavigatedComponent.Operand } : new IQueryComponent[0]);
        }
        #endregion
    }
}