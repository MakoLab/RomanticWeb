using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Expressions;
using RomanticWeb.Linq.Model;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Linq
{
    internal static class QueryVisitorExtensions
    {
        private static readonly IDictionary<object, object> TransformedExpressionsCache = new Dictionary<object, object>();

        internal static StrongEntityAccessor GetEntityAccessor(this IQueryVisitor visitor, Remotion.Linq.Clauses.FromClauseBase sourceExpression)
        {
            StrongEntityAccessor entityAccessor = null;
            if (typeof(IEntity).IsAssignableFrom(sourceExpression.ItemType))
            {
                sourceExpression = visitor.TransformFromExpression(sourceExpression);
                entityAccessor = visitor.Query.FindAllComponents<StrongEntityAccessor>().FirstOrDefault(item => item.SourceExpression.EqualsTo(sourceExpression));
                if (entityAccessor == null)
                {
                    Identifier identifier = visitor.Query.FindAllComponents<EntityConstrain>()
                        .Where(item => (item.TargetExpression.EqualsTo(sourceExpression.FromExpression)) && (item.Value is Identifier))
                        .Select(item => (Identifier)item.Value)
                        .FirstOrDefault() ?? new Identifier(visitor.Query.CreateVariableName(sourceExpression.ItemName), sourceExpression.ItemType.FindEntityType());
                    entityAccessor = new StrongEntityAccessor(identifier, sourceExpression);
                    entityAccessor.UnboundGraphName = entityAccessor.About;
                    EntityTypeConstrain constrain = visitor.CreateTypeConstrain(sourceExpression);
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
                    IEnumerable<Type> inheritedTypes = entityType.GetImplementingTypes();
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

        internal static Remotion.Linq.Clauses.FromClauseBase TransformFromExpression(this IQueryVisitor visitor, Remotion.Linq.Clauses.FromClauseBase sourceExpression)
        {
            Remotion.Linq.Clauses.FromClauseBase result = sourceExpression;
            System.Linq.Expressions.Expression expression = visitor.TransformUnaryExpression(sourceExpression.FromExpression);
            if (expression != sourceExpression.FromExpression)
            {
                object item;
                if (!TransformedExpressionsCache.TryGetValue(sourceExpression, out item))
                {
                    TransformedExpressionsCache[sourceExpression] = result =
                        new RomanticWeb.Linq.Expressions.FromPropertyClause(sourceExpression.ItemName, sourceExpression.ItemType, (System.Linq.Expressions.MemberExpression)expression);
                }
                else
                {
                    result = (RomanticWeb.Linq.Expressions.FromPropertyClause)item;
                }
            }

            return result;
        }

        internal static System.Linq.Expressions.Expression TransformUnaryExpression(this IQueryVisitor visitor, System.Linq.Expressions.Expression sourceExpression)
        {
            System.Linq.Expressions.Expression result = sourceExpression;
            if (sourceExpression is System.Linq.Expressions.UnaryExpression)
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)sourceExpression;
                if (unaryExpression.NodeType == System.Linq.Expressions.ExpressionType.TypeAs)
                {
                    System.Linq.Expressions.Expression expression = visitor.TransformPredicateExpression(unaryExpression.Operand);
                    if (expression != sourceExpression)
                    {
                        object item;
                        if (!TransformedExpressionsCache.TryGetValue(sourceExpression, out item))
                        {
                            TransformedExpressionsCache[sourceExpression] = result = expression;
                        }
                        else
                        {
                            result = (System.Linq.Expressions.Expression)item;
                        }
                    }
                }
            }

            return result;
        }

        internal static System.Linq.Expressions.Expression TransformPredicateExpression(this IQueryVisitor visitor, System.Linq.Expressions.Expression expression)
        {
            System.Linq.Expressions.Expression result = expression;
            if (expression is System.Linq.Expressions.MethodCallExpression)
            {
                System.Linq.Expressions.MethodCallExpression methodCallExpression = (System.Linq.Expressions.MethodCallExpression)expression;
                if ((methodCallExpression.Method == typeof(EntityExtensions).GetMethod("Predicate")) && (methodCallExpression.Arguments.Count > 1) &&
                    (methodCallExpression.Arguments[1] != null) && (methodCallExpression.Arguments[1] is System.Linq.Expressions.ConstantExpression))
                {
                    object item;
                    if (!TransformedExpressionsCache.TryGetValue(expression, out item))
                    {
                        object objectValue = ((System.Linq.Expressions.ConstantExpression)methodCallExpression.Arguments[1]).Value;
                        Uri predicate = (Uri)objectValue;
                        Type type;
                        string name;
                        visitor.GetMappingDetails(predicate, methodCallExpression.Arguments[0].Type, out type, out name);
                        System.Linq.Expressions.MemberExpression memberExpression = System.Linq.Expressions.Expression.MakeMemberAccess(methodCallExpression.Arguments[0], type.GetProperty(name));
                        TransformedExpressionsCache[expression] = result = memberExpression;
                    }
                    else
                    {
                        result = (System.Linq.Expressions.MemberExpression)item;
                    }
                }
            }

            return result;
        }

        private static void GetMappingDetails(this IQueryVisitor visitor, Uri predicate, Type suggestedType, out Type itemType, out string itemName)
        {
            if (!predicate.IsAbsoluteUri)
            {
                predicate = new Uri(visitor.BaseUriSelector.SelectBaseUri(new EntityId(predicate)), predicate.ToString());
            }

            itemType = null;
            itemName = null;
            if (predicate == Rdf.subject)
            {
                itemName = "Id";
                itemType = typeof(IEntity);
            }
            else
            {
                IPropertyMapping propertyMapping = null;
                if (suggestedType != null)
                {
                    IEntityMapping entityMapping = visitor.MappingsRepository.MappingFor(suggestedType);
                    if (entityMapping != null)
                    {
                        propertyMapping = entityMapping.Properties.FirstOrDefault(item => item.Uri.AbsoluteUri == predicate.AbsoluteUri); 
                    }
                }

                if (propertyMapping == null)
                {
                    propertyMapping = visitor.MappingsRepository.MappingForProperty(predicate);
                }

                if (propertyMapping == null)
                {
                    ExceptionHelper.ThrowMappingException(predicate);
                }

                itemName = propertyMapping.Name;
                itemType = propertyMapping.EntityMapping.EntityType;
            }
        }
    }
}
