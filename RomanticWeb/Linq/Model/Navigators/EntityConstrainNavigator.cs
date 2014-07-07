using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates entity constrain.</summary>
    internal class EntityConstrainNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated entity constrain.</summary>
        /// <param name="entityConstrain">Nagivated entity constrain.</param>
        internal EntityConstrainNavigator(EntityConstrain entityConstrain)
            : base(entityConstrain)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public EntityConstrain NavigatedComponent { get { return (EntityConstrain)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is IExpression) && ((NavigatedComponent.Predicate == null) || (NavigatedComponent.Value == null));
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (NavigatedComponent.Predicate == component) || (NavigatedComponent.Value == component);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is IExpression)
            {
                if (NavigatedComponent.Predicate == null)
                {
                    NavigatedComponent.Predicate = (IExpression)component;
                }
                else if (NavigatedComponent.Value == null)
                {
                    NavigatedComponent.Value = (IExpression)component;
                }
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is IExpression) && (replacement is IExpression))
            {
                if (NavigatedComponent.Predicate == (IExpression)component)
                {
                    NavigatedComponent.Predicate = (IExpression)replacement;
                }
                else if (NavigatedComponent.Value == (IExpression)component)
                {
                    NavigatedComponent.Value = (IExpression)component;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result = new List<IQueryComponent>();
            if (NavigatedComponent.Predicate != null)
            {
                result.Add(NavigatedComponent.Predicate);
            }

            if (NavigatedComponent.Value != null)
            {
                result.Add(NavigatedComponent.Value);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}