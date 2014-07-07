using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates calls.</summary>
    internal class CallNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated call.</summary>
        /// <param name="call">Nagivated call.</param>
        internal CallNavigator(Call call)
            : base(call)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public Call NavigatedComponent { get { return (Call)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is IExpression);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (component is IExpression ? NavigatedComponent.Arguments.Contains((IExpression)component) : false);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is IExpression)
            {
                NavigatedComponent.Arguments.Add((IExpression)component);
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is IExpression) && (replacement is IExpression))
            {
                int indexOf = NavigatedComponent.Arguments.IndexOf((IExpression)component);
                if (indexOf != -1)
                {
                    NavigatedComponent.Arguments.RemoveAt(indexOf);
                    NavigatedComponent.Arguments.Insert(indexOf, (IExpression)replacement);
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            return new List<IQueryComponent>(NavigatedComponent.Arguments).AsReadOnly();
        }
        #endregion
    }
}