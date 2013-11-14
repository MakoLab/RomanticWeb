using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates entity accessor.</summary>
    internal class EntityAccessorNavigator:QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated entity accessor.</summary>
        /// <param name="entityAccessor">Nagivated entity accessor.</param>
        internal EntityAccessorNavigator(EntityAccessor entityAccessor):base(entityAccessor)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public EntityAccessor NavigatedComponent { get { return (EntityAccessor)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is QueryElement)||((component is Identifier)&&(NavigatedComponent.About==null));
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (component is QueryElement?NavigatedComponent.Elements.Contains((QueryElement)component):
                (component is Identifier?(NavigatedComponent.About==component):false));
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
                Identifier identifier=(Identifier)component;
                if (NavigatedComponent.About==null)
                {
                    NavigatedComponent.About=identifier;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result=new List<IQueryComponent>(NavigatedComponent.Elements);
            if (NavigatedComponent.About!=null)
            {
                result.Add(NavigatedComponent.About);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}