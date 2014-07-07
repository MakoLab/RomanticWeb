using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Model.Navigators;
using RomanticWeb.Mapping;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides useful query component extension methods.</summary>
    public static class QueryComponentExtensions
    {
        /// <summary>Searches the query object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="query">Query to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(this Query query) where T : IQueryComponent
        {
            return query.GetQueryComponentNavigator().FindAllComponents<T>(new List<Query>());
        }

        /// <summary>Searches the entity accessor object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="entityAccessor">Entity accessor to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(this StrongEntityAccessor entityAccessor) where T : IQueryComponent
        {
            return entityAccessor.GetQueryComponentNavigator().FindAllComponents<T>(new List<Query>());
        }

        /// <summary>Searches the query object graph for specific items.</summary>
        /// <typeparam name="T">Type of the item to be searched for.</typeparam>
        /// <param name="queryComponentNavigator">Query component navigator to be searched through.</param>
        /// <remarks>This method performs a deep search for all items that match given type and its derivatives.</remarks>
        /// <returns>Enumeration of found components matching given type.</returns>
        internal static IEnumerable<T> FindAllComponents<T>(this IQueryComponentNavigator queryComponentNavigator) where T : IQueryComponent
        {
            return FindAllComponents<T>(queryComponentNavigator, new List<Query>());
        }

        /// <summary>Gets a query component navigator attribute for given query component.</summary>
        /// <param name="queryComponent">Query component to be inspected.</param>
        /// <returns><see cref="QueryComponentNavigatorAttribute" /> or null.</returns>
        internal static QueryComponentNavigatorAttribute GetQueryComponentNavigatorAttribute(this IQueryComponent queryComponent)
        {
            QueryComponentNavigatorAttribute result = null;
            object[] attributes = queryComponent.GetType().GetCustomAttributes(typeof(QueryComponentNavigatorAttribute), true);
            if (attributes.Length > 0)
            {
                result = (QueryComponentNavigatorAttribute)attributes[0];
            }

            return result;
        }

        /// <summary>Converts a query component navigator into the query component itself.</summary>
        /// <param name="queryComponentNavigator">Query component navigator to be converted.</param>
        /// <returns>Query component.</returns>
        internal static IQueryComponent GetQueryComponent(this IQueryComponentNavigator queryComponentNavigator)
        {
            IQueryComponentNavigator _this = (IQueryComponentNavigator)queryComponentNavigator;
            if (!(_this.NavigatedComponent is QueryComponent))
            {
                throw new InvalidOperationException(System.String.Format("Cannot convert to query component objects of type '{0}'.", _this.NavigatedComponent.GetType()));
            }

            return (QueryComponent)_this.NavigatedComponent;
        }

        /// <summary>Converts a query component into its navigator.</summary>
        /// <param name="queryComponent">Query component to be converted.</param>
        /// <returns>Query component navigator or null.</returns>
        [return: AllowNull]
        internal static IQueryComponentNavigator GetQueryComponentNavigator(this IQueryComponent queryComponent)
        {
            IQueryComponentNavigator result = null;
            QueryComponentNavigatorAttribute queryComponentNavigatorAttribute = queryComponent.GetQueryComponentNavigatorAttribute();
            if (queryComponentNavigatorAttribute != null)
            {
                result = (IQueryComponentNavigator)queryComponentNavigatorAttribute.Constructor.Invoke(new object[] { queryComponent });
            }

            return result;
        }

        internal static StrongEntityAccessor GetEntityAccessor(this IQueryVisitor visitor, Remotion.Linq.Clauses.FromClauseBase sourceExpression)
        {
            StrongEntityAccessor entityAccessor = null;
            if (typeof(IEntity).IsAssignableFrom(sourceExpression.ItemType))
            {
                entityAccessor = visitor.Query.FindAllComponents<StrongEntityAccessor>()
                    .Where(item => (item.SourceExpression != null) && (item.SourceExpression.FromExpression == sourceExpression.FromExpression)).FirstOrDefault();
                if (entityAccessor == null)
                {
                    EntityTypeConstrain constrain = visitor.CreateTypeConstrain(sourceExpression);
                    Identifier identifier = new Identifier(visitor.Query.CreateVariableName(sourceExpression.ItemName), sourceExpression.ItemType.FindEntityType());
                    entityAccessor = new StrongEntityAccessor(identifier, sourceExpression);
                    if ((constrain != null) && (!entityAccessor.Elements.Contains(constrain)))
                    {
                        entityAccessor.Elements.Add(constrain);
                    }
                }
            }

            return entityAccessor;
        }

        internal static EntityTypeConstrain CreateTypeConstrain(this IQueryVisitor visitor, Remotion.Linq.Clauses.FromClauseBase sourceExpression)
        {
            EntityTypeConstrain result = null;
            Type entityType = sourceExpression.ItemType.FindEntityType();
            if ((entityType != null) && (entityType != typeof(IEntity)))
            {
                result = visitor.CreateTypeConstrain(entityType, sourceExpression.FromExpression);
            }

            return result;
        }

        internal static EntityTypeConstrain CreateTypeConstrain(this IQueryVisitor visitor, Type entityType, System.Linq.Expressions.Expression sourceExpression)
        {
            EntityTypeConstrain result = null;
            if (entityType != null)
            {
                var classMappings = visitor.MappingsRepository.FindMappedClasses(entityType);

                if (classMappings == null)
                {
                    throw new UnMappedTypeException(entityType);
                }

                if (classMappings.Any())
                {
                    Uri primaryTypeUri = classMappings.First();
                    IEnumerable<Type> inheritedTypes = ContainerFactory.GetTypesImplementing(entityType);
                    IList<Uri> inheritedTypeUris = new List<Uri>();
                    if (inheritedTypes.Any())
                    {
                        foreach (Type inheritedType in inheritedTypes)
                        {
                            classMappings = visitor.MappingsRepository.FindMappedClasses(inheritedType);

                            if (classMappings == null)
                            {
                                throw new UnMappedTypeException(entityType);
                            }

                            if (classMappings.Any())
                            {
                                Uri inheritedTypeUri = classMappings.First();
                                if ((primaryTypeUri.AbsoluteUri != inheritedTypeUri.AbsoluteUri) && (!inheritedTypeUris.Contains(inheritedTypeUri, AbsoluteUriComparer.Default)))
                                {
                                    inheritedTypeUris.Add(inheritedTypeUri);
                                }
                            }
                        }
                    }

                    result = new EntityTypeConstrain(primaryTypeUri, sourceExpression, inheritedTypeUris.ToArray());
                }
            }

            return result;
        }

        private static IEnumerable<T> FindAllComponents<T>(this IQueryComponentNavigator queryComponentNavigator, IList<Query> searchedQueries) where T : IQueryComponent
        {
            List<T> result = new List<T>();
            Query query = null;
            if (queryComponentNavigator is QueryNavigator)
            {
                query = (Query)queryComponentNavigator.GetQueryComponent();
                if ((!searchedQueries.Contains(query)) && (searchedQueries.Count < 2))
                {
                    searchedQueries.Add(query);
                    if ((query != null) && (query.OwnerQuery != null))
                    {
                        result.AddUniqueRange(query.OwnerQuery.GetQueryComponentNavigator().FindAllComponents<T>(searchedQueries));
                    }
                }
                else
                {
                    query = null;
                }
            }

            foreach (IQueryComponent component in queryComponentNavigator.GetComponents())
            {
                if ((typeof(T).IsAssignableFrom(component.GetType())) && (!result.Contains((T)component)))
                {
                    result.Add((T)component);
                }

                IQueryComponentNavigator componentNavigator = component.GetQueryComponentNavigator();
                if ((componentNavigator != null) && ((!(component is Query)) || (!searchedQueries.Contains((Query)component))))
                {
                    result.AddUniqueRange(componentNavigator.FindAllComponents<T>(searchedQueries));
                }
            }

            return result.AsReadOnly();
        }

        private static void AddUniqueRange(this IList list, IEnumerable range)
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