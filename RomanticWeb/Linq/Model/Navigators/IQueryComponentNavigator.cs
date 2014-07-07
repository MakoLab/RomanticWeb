using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Provides a base interface for query component navigators that can have child elements.</summary>
    public interface IQueryComponentNavigator
    {
        /// <summary>Gets a navigated component.</summary>
        IQueryComponent NavigatedComponent { get; }

        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        bool CanAddComponent(IQueryComponent component);

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        bool ContainsComponent(IQueryComponent component);

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        void AddComponent(IQueryComponent component);

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        void ReplaceComponent(IQueryComponent component, IQueryComponent replacement);

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        IEnumerable<IQueryComponent> GetComponents();
    }
}