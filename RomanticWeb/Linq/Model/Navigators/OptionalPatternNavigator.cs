using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates optional patterns.</summary>
    internal class OptionalPatternNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated optional patterns.</summary>
        /// <param name="optionalPattern">Nagivated optional patterns.</param>
        internal OptionalPatternNavigator(OptionalPattern optionalPattern)
            : base(optionalPattern)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public OptionalPattern NavigatedComponent { get { return (OptionalPattern)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is EntityConstrain);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (component is EntityConstrain ? NavigatedComponent.Patterns.Contains((EntityConstrain)component) : false);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is EntityConstrain)
            {
                NavigatedComponent.Patterns.Add((EntityConstrain)component);
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is EntityConstrain) && (replacement is EntityConstrain))
            {
                int indexOf = NavigatedComponent.Patterns.IndexOf((EntityConstrain)component);
                if (indexOf != -1)
                {
                    NavigatedComponent.Patterns.RemoveAt(indexOf);
                    NavigatedComponent.Patterns.Insert(indexOf, (EntityConstrain)replacement);
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            return new List<IQueryComponent>(NavigatedComponent.Patterns).AsReadOnly();
        }
        #endregion
    }
}