using System;
using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates entity type constrain.</summary>
    internal class EntityTypeConstrainNavigator:QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated entity type constrain.</summary>
        /// <param name="entityTypeConstrain">Nagivated entity typeconstrain.</param>
        internal EntityTypeConstrainNavigator(EntityTypeConstrain entityTypeConstrain):base(entityTypeConstrain)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public EntityTypeConstrain NavigatedComponent { get { return (EntityTypeConstrain)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is Literal)&&(((Literal)component).Value is Uri)&&(NavigatedComponent.Value==null);
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (NavigatedComponent.Value==component);
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if ((component is Literal)&&(((Literal)component).Value is Uri)&&(NavigatedComponent.Value==null))
            {
                NavigatedComponent.Value=(Literal)component;
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component,IQueryComponent replacement)
        {
            if ((component is Literal)&&(replacement is Literal)&&(((Literal)component).Value is Uri)&&(((Literal)replacement).Value is Uri))
            {
                if (NavigatedComponent.Value==(Literal)component)
                {
                    NavigatedComponent.Value=(Literal)component;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            return (NavigatedComponent.Value!=null?new IQueryComponent[] { (Literal)NavigatedComponent.Value } :new IQueryComponent[0]);
        }
        #endregion
    }
}