using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates unspecified entity accessor.</summary>
    internal class UnspecifiedEntityAccessorNavigator : QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated unspecified entity accessor.</summary>
        /// <param name="entityAccessor">Nagivated entity accessor.</param>
        internal UnspecifiedEntityAccessorNavigator(UnspecifiedEntityAccessor entityAccessor)
            : base(entityAccessor)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public UnspecifiedEntityAccessor NavigatedComponent { get { return (UnspecifiedEntityAccessor)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is QueryElement) || ((component is Identifier) && (NavigatedComponent.About == null));
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (component is QueryElement ? NavigatedComponent.Elements.Contains((QueryElement)component) :
                (component is Identifier ? (NavigatedComponent.About == component) : false));
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is QueryElement)
            {
                NavigatedComponent.Elements.Add((QueryElement)component);
            }
            else if (component is Identifier)
            {
                Identifier identifier = (Identifier)component;
                if (NavigatedComponent.About == null)
                {
                    NavigatedComponent.About = identifier;
                }
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component, IQueryComponent replacement)
        {
            if ((component is QueryElement) && (replacement is QueryElement))
            {
                int indexOf = NavigatedComponent.Elements.IndexOf((QueryElement)component);
                if (indexOf != -1)
                {
                    NavigatedComponent.Elements.RemoveAt(indexOf);
                    NavigatedComponent.Elements.Insert(indexOf, (QueryElement)replacement);
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result = new List<IQueryComponent>(NavigatedComponent.EntityAccessor.Elements);

            if (NavigatedComponent.About != null)
            {
                result.Add(NavigatedComponent.About);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}