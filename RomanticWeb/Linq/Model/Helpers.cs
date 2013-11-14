using System;
using System.Collections;
using System.Collections.Generic;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides useful query component extension methods.</summary>
    public static class Helpers
    {
        /// <summary>Checks if a query component has a navigator.</summary>
        /// <param name="queryComponent">Query component to be checked</param>
        /// <returns><b>true</b> if the query component has a navigator, otherwise <b>false</b>.</returns>
        internal static bool CheckNavigator(IQueryComponent queryComponent)
        {
            QueryComponentNavigatorAttribute queryComponentNavigatorAttribute=GetQueryComponentNavigatorAttribute(queryComponent);
            return (queryComponentNavigatorAttribute!=null);
        }

        /// <summary>Searches the query object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="query">Query to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(Query query) where T:IQueryComponent
        {
            return FindAllComponents<T>(GetQueryComponentNavigator(query),new List<Query>());
        }

        /// <summary>Searches the entity accessor object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="entityAccessor">Entity accessor to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(EntityAccessor entityAccessor) where T:IQueryComponent
        {
            return FindAllComponents<T>(GetQueryComponentNavigator(entityAccessor),new List<Query>());
        }

        /// <summary>Searches the query object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="queryComponentNavigator">Query component navigator to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(IQueryComponentNavigator queryComponentNavigator) where T:IQueryComponent
        {
            return FindAllComponents<T>(queryComponentNavigator,new List<Query>());
        }

        /// <summary>Gets a query component navigator attribute for given query component.</summary>
        /// <param name="queryComponent">Query component to be inspected.</param>
        /// <returns><see cref="QueryComponentNavigatorAttribute" /> or null.</returns>
        internal static QueryComponentNavigatorAttribute GetQueryComponentNavigatorAttribute(IQueryComponent queryComponent)
        {
            QueryComponentNavigatorAttribute result=null;
            object[] attributes=queryComponent.GetType().GetCustomAttributes(typeof(QueryComponentNavigatorAttribute),true);
            if (attributes.Length>0)
            {
                result=(QueryComponentNavigatorAttribute)attributes[0];
            }

            return result;
        }

        /// <summary>Converts a query component navigator into the query component itself.</summary>
        /// <param name="queryComponentNavigator">Query component navigator to be converted.</param>
        /// <returns>Query component.</returns>
        internal static IQueryComponent GetQueryComponent(IQueryComponentNavigator queryComponentNavigator)
        {
            IQueryComponentNavigator _this=(IQueryComponentNavigator)queryComponentNavigator;
            if (!(_this.NavigatedComponent is QueryComponent))
            {
                throw new InvalidOperationException(System.String.Format("Cannot convert to query component objects of type '{0}'.",_this.NavigatedComponent.GetType()));
            }

            return (QueryComponent)_this.NavigatedComponent;
        }

        /// <summary>Converts a query component into its navigator.</summary>
        /// <param name="queryComponent">Query component to be converted.</param>
        /// <returns>Query component navigator or null.</returns>
        [return: AllowNull]
        internal static IQueryComponentNavigator GetQueryComponentNavigator(IQueryComponent queryComponent)
        {
            IQueryComponentNavigator result=null;
            QueryComponentNavigatorAttribute queryComponentNavigatorAttribute=Helpers.GetQueryComponentNavigatorAttribute(queryComponent);
            if (queryComponentNavigatorAttribute!=null)
            {
                result=(IQueryComponentNavigator)queryComponentNavigatorAttribute.Constructor.Invoke(new object[] { queryComponent });
            }

            return result;
        }

        private static IEnumerable<T> FindAllComponents<T>(this IQueryComponentNavigator queryComponentNavigator,IList<Query> searchedQueries) where T:IQueryComponent
        {
            List<T> result=new List<T>();
            Query query=null;
            if (queryComponentNavigator is QueryNavigator)
            {
                query=(Query)GetQueryComponent(queryComponentNavigator);
                if ((!searchedQueries.Contains(query))&&(searchedQueries.Count<2))
                {
                    searchedQueries.Add(query);
                    if ((query!=null)&&(query.OwnerQuery!=null))
                    {
                        result.AddUniqueRange(GetQueryComponentNavigator(query.OwnerQuery).FindAllComponents<T>(searchedQueries));
                    }
                }
                else
                {
                    query=null;
                }
            }

            foreach (IQueryComponent component in queryComponentNavigator.GetComponents())
            {
                if ((typeof(T).IsAssignableFrom(component.GetType()))&&(!result.Contains((T)component)))
                {
                    result.Add((T)component);
                }

                IQueryComponentNavigator componentNavigator=GetQueryComponentNavigator(component);
                if ((componentNavigator!=null)&&((!(component is Query))||(!searchedQueries.Contains((Query)component))))
                {
                    result.AddUniqueRange(componentNavigator.FindAllComponents<T>(searchedQueries));
                }
            }

            return result.AsReadOnly();
        }

        private static void AddUniqueRange(this IList list,IEnumerable range)
        {
            foreach (object item in range)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
        }
    }
}