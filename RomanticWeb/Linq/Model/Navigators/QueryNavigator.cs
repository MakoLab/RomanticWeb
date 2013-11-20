using System.Collections.Generic;

namespace RomanticWeb.Linq.Model.Navigators
{
    /// <summary>Navigates query.</summary>
    internal class QueryNavigator:QueryComponentNavigatorBase
    {
        #region Constructors
        /// <summary>Default constructor with nagivated query.</summary>
        /// <param name="query">Nagivated query.</param>
        internal QueryNavigator(Query query):base(query)
        {
        }
        #endregion

        #region Properties
        /// <summary>Gets a navigated component.</summary>
        public Query NavigatedComponent { get { return (Query)((IQueryComponentNavigator)this).NavigatedComponent; } }
        #endregion

        #region Public methods
        /// <summary>Determines if the given component can accept another component as a child.</summary>
        /// <param name="component">Component to be added.</param>
        /// <returns><b>true</b> if given component can be added, otherwise <b>false</b>.</returns>
        public override bool CanAddComponent(IQueryComponent component)
        {
            return (component is QueryElement)||(component is Prefix)||(component is IExpression)||((component is Identifier)&&(NavigatedComponent.Subject==null));
        }

        /// <summary>Determines if the given component contains another component as a child.</summary>
        /// <param name="component">Component to be checked.</param>
        /// <returns><b>true</b> if given component is already contained, otherwise <b>false</b>.</returns>
        public override bool ContainsComponent(IQueryComponent component)
        {
            return (component is QueryElement?NavigatedComponent.Elements.Contains((QueryElement)component):
                (component is Prefix?NavigatedComponent.Prefixes.Contains((Prefix)component):
                    (component is ISelectableQueryComponent?NavigatedComponent.Select.Contains((ISelectableQueryComponent)component):
                        (component is Identifier?NavigatedComponent.Subject==component:false))));
        }

        /// <summary>Adds component as a child of another component.</summary>
        /// <param name="component">Component to be added.</param>
        public override void AddComponent(IQueryComponent component)
        {
            if (component is QueryElement)
            {
                NavigatedComponent.Elements.Add((QueryElement)component);
            }
            else if (component is Prefix)
            {
                NavigatedComponent.Prefixes.Add((Prefix)component);
            }
            else if (component is ISelectableQueryComponent)
            {
                NavigatedComponent.Select.Add((ISelectableQueryComponent)component);
            }
            else if ((component is Identifier)&&(NavigatedComponent.Subject==null))
            {
                NavigatedComponent.Subject=(Identifier)component;
            }
        }

        /// <summary>Replaces given component with another component.</summary>
        /// <param name="component">Component to be replaced.</param>
        /// <param name="replacement">Component to be put instead.</param>
        public override void ReplaceComponent(IQueryComponent component,IQueryComponent replacement)
        {
            if ((component is QueryElement)&&(replacement is QueryElement))
            {
                int indexOf=NavigatedComponent.Elements.IndexOf((QueryElement)component);
                if (indexOf!=-1)
                {
                    NavigatedComponent.Elements.RemoveAt(indexOf);
                    NavigatedComponent.Elements.Insert(indexOf,(QueryElement)replacement);
                }
            }
            else if ((component is Prefix)&&(replacement is Prefix))
            {
                int indexOf=NavigatedComponent.Prefixes.IndexOf((Prefix)component);
                if (indexOf!=-1)
                {
                    NavigatedComponent.Prefixes.RemoveAt(indexOf);
                    NavigatedComponent.Prefixes.Insert(indexOf,(Prefix)replacement);
                }
            }
            else if ((component is ISelectableQueryComponent)&&(replacement is ISelectableQueryComponent))
            {
                int indexOf=NavigatedComponent.Select.IndexOf((ISelectableQueryComponent)component);
                if (indexOf!=-1)
                {
                    NavigatedComponent.Select.RemoveAt(indexOf);
                    NavigatedComponent.Select.Insert(indexOf,(ISelectableQueryComponent)replacement);
                }
            }
            else if ((component is Identifier)&&(replacement is Identifier))
            {
                if (NavigatedComponent.Subject==(Identifier)component)
                {
                    NavigatedComponent.Subject=(Identifier)replacement;
                }
            }
        }

        /// <summary>Retrieves all child components.</summary>
        /// <returns>Enumeration of all child components.</returns>
        public override IEnumerable<IQueryComponent> GetComponents()
        {
            List<IQueryComponent> result=new List<IQueryComponent>(NavigatedComponent.Elements);
            result.AddRange(NavigatedComponent.Prefixes);
            result.AddRange(NavigatedComponent.Select);
            if (NavigatedComponent.Subject!=null)
            {
                result.Add(NavigatedComponent.Subject);
            }

            return result.AsReadOnly();
        }
        #endregion
    }
}