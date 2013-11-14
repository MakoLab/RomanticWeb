using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates filters.</summary>
    internal class FilterNavigator:QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated filter.</summary>
        /// <param name="filter">Nagivated filter.</param>
        internal FilterNavigator(Filter filter):base(filter)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public Filter NavigatedComponent { get { return (Filter)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is IExpression)&&(NavigatedComponent.Expression==null);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (NavigatedComponent.Expression==component);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if ((component is IExpression)&&(NavigatedComponent.Expression==null))
            {
                NavigatedComponent.Expression=(IExpression)component;
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            return (NavigatedComponent.Expression!=null?new IQueryComponent[] { NavigatedComponent.Expression } :new IQueryComponent[0]);
        }
        #endregion
    }
}