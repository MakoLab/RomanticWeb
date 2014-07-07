using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates an alias.</summary>
    internal class AliasNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated alias.</summary>
        /// <param name="alias">Nagivated alias.</param>
        internal AliasNavigator(Alias alias)
            : base(alias)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public Alias NavigatedComponent { get { return (Alias)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return ((component is Identifier) && (NavigatedComponent.Name == null)) || (NavigatedComponent.Component == null);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (NavigatedComponent.Name == component) || (NavigatedComponent.Component == component);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is Identifier)
            {
                if (NavigatedComponent.Name == null)
                {
                    NavigatedComponent.Name = (Identifier)component;
                }
            }
            else if (NavigatedComponent.Component == null)
            {
                NavigatedComponent.Component = component;
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is Identifier) && (replacement is Identifier))
            {
                if (NavigatedComponent.Name == (Identifier)component)
                {
                    NavigatedComponent.Name = (Identifier)replacement;
                }
            }
            else if (NavigatedComponent.Component == component)
            {
                NavigatedComponent.Component = replacement;
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result = new List<IQueryComponent>();
            if (NavigatedComponent.Name != null)
            {
                result.Add(NavigatedComponent.Name);
            }

            if (NavigatedComponent.Component != null)
            {
                result.Add(NavigatedComponent.Component);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}