using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates binary operators.</summary>
    internal class BinaryOperatorNavigator:QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated binary operator.</summary>
        /// <param name="binaryOperator">Nagivated binary operator.</param>
        internal BinaryOperatorNavigator(BinaryOperator binaryOperator):base(binaryOperator)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public BinaryOperator NavigatedComponent { get { return (BinaryOperator)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is IExpression)&&((NavigatedComponent.LeftOperand==null)||(NavigatedComponent.RightOperand==null));
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return ((NavigatedComponent.LeftOperand==component)||(NavigatedComponent.RightOperand==component));
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is IExpression)
            {
                if (NavigatedComponent.LeftOperand==null)
                {
                    NavigatedComponent.LeftOperand=(IExpression)component;
                }
                else if (NavigatedComponent.RightOperand==null)
                {
                    NavigatedComponent.RightOperand=(IExpression)component;
                }
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component,IQueryComponent replacement)
        {
            if ((component is IExpression)&&(replacement is IExpression))
            {
                if (NavigatedComponent.LeftOperand==(IExpression)component)
                {
                    NavigatedComponent.LeftOperand=(IExpression)replacement;
                }
                else if (NavigatedComponent.RightOperand==(IExpression)component)
                {
                    NavigatedComponent.RightOperand=(IExpression)replacement;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result=new List<IQueryComponent>();
            if (NavigatedComponent.LeftOperand!=null)
            {
                result.Add(NavigatedComponent.LeftOperand);
            }

            if (NavigatedComponent.RightOperand!=null)
            {
                result.Add(NavigatedComponent.RightOperand);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}