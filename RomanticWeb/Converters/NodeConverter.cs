using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Default converter for <see cref="Node"/>s to value objects or entities.</summary>
    public sealed class NodeConverter:INodeConverter
    {
        private readonly IEntityContext _entityContext;

        /// <summary>Constructor with entity context passed.</summary>
        /// <param name="entityContext">Entity context to be used.</param>
        public NodeConverter(IEntityContext entityContext)
        {
            _entityContext=entityContext;
            Converters=new List<ILiteralNodeConverter>();
            ComplexTypeConverters=new List<IComplexTypeConverter>();
        }

        /// <summary>Gets literal node converters.</summary>
        [ImportMany]
        public IEnumerable<ILiteralNodeConverter> Converters { get; internal set; }

        /// <summary>Gets complex type converters.</summary>
        [ImportMany]
        public IEnumerable<IComplexTypeConverter> ComplexTypeConverters { get; internal set; }

        /// <summary>Converts <see cref="Node"/>s and checks for validity against destination property mapping.</summary>
        /// <remarks>
        ///     <ul>
        ///         <li>Returns typed instances of <see cref="Entity"/> based on property's return value</li>
        ///         <li>Doesn't check the type of literals against the property's return type</li>
        ///     </ul>
        /// </remarks>
        public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects,[AllowNull] IPropertyMapping propertyMapping)
        {
            foreach (var objectNode in objects.ToList())
            {
                if (objectNode.IsLiteral)
                {
                    yield return ConvertLiteral(objectNode,propertyMapping);
                }
                else
                {
                    yield return ConvertEntity(objectNode,propertyMapping);
                }
            }
        }

        /// <summary>Converts <see cref="Node"/>s to most appropriate type based on raw RDF data.</summary>
        /// <remarks>This will always return untyped instanes of <see cref="Entity"/> for URI nodes.</remarks>
        public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects)
        {
            return ConvertNodes(objects,null);
        }

        /// <summary>Converts a value to nodes.</summary>
        public IEnumerable<Node> ConvertBack(object value,IPropertyMapping property)
        {
            var convertedNodes=new List<Node>();

            if (typeof(IEntity).IsAssignableFrom(property.ReturnType))
            {
                convertedNodes.Add(ConvertOneBack(value));
            }

            if (typeof(IEnumerable<IEntity>).IsAssignableFrom(property.ReturnType))
            {
                var convertedEntities=from entity in ((IEnumerable)value).Cast<IEntity>()
                                      select ConvertOneBack(entity);
                convertedNodes.AddRange(convertedEntities);
            }

            if (property.IsCollection)
            {
                var objectsToConvert=from obj in ((IEnumerable)value).Cast<object>()
                                     where obj.GetType()==property.ReturnType.GetGenericArguments().First()
                                     select obj;
                foreach (var element in objectsToConvert)
                {
                    var converter=ComplexTypeConverters.FirstOrDefault(c => c.CanConvertBack(element,property));
                    if (converter!=null)
                    {
                        convertedNodes.AddRange(converter.ConvertBack(element));
                    }
                    else
                    {
                        convertedNodes.Add(ConvertOneBack(element));
                    }
                }
            }
            else
            {
                convertedNodes.Add(ConvertOneBack(value));
            }

            return convertedNodes;
        }

        private static bool PredicateIsEntityOrCollectionThereof(IPropertyMapping predicate,out Type entityType)
        {
            entityType=null;
            if (predicate==null)
            {
                return false;
            }

            if (typeof(IEntity).IsAssignableFrom(predicate.ReturnType))
            {
                entityType=predicate.ReturnType;
                return true;
            }

            if (typeof(IEnumerable<IEntity>).IsAssignableFrom(predicate.ReturnType))
            {
                entityType=predicate.ReturnType.GenericTypeArguments.Single();
                return true;
            }

            return false;
        }

        private Node ConvertOneBack(object element)
        {
            if (element is IEntity)
            {
                return Node.FromEntityId(((IEntity)element).Id);
            }

            // todo: this is a hack, and should be in a complex type converter
            if (element is Uri)
            {
                return Node.ForUri((Uri)element);
            }

            return Node.ForLiteral(element.ToString());
        }

        private object ConvertLiteral(Node objectNode,IPropertyMapping propertyMapping)
        {
            if ((propertyMapping != null)&&(propertyMapping.ReturnType==typeof(String)))
            {
                return objectNode.Literal;
            }

            var converter=GetBestConverter(objectNode);
            if (converter!=null)
            {
                return converter.Convert(objectNode);
            }

            return objectNode.Literal;
        }

        private object ConvertEntity(Node objectNode,IPropertyMapping predicate)
        {
            IEntity entity;
            if ((predicate==null)||(!typeof(EntityId).IsAssignableFrom(predicate.ReturnType.FindItemType())))
            {
                entity=_entityContext.Load<IEntity>(objectNode.ToEntityId(),false);
            }
            else
            {
                entity=new Entity(objectNode.ToEntityId());
            }

            var converter=ComplexTypeConverters.FirstOrDefault(c => c.CanConvert(entity,_entityContext.Store,predicate));
            if (converter!=null)
            {
                return converter.Convert(entity,_entityContext.Store,predicate);
            }

            Type entityType;
            if (PredicateIsEntityOrCollectionThereof(predicate,out entityType))
            {
                Type itemType=predicate.ReturnType.FindItemType();
                if ((!entityType.IsAssignableFrom(itemType))||((itemType==entityType)&&(!entityType.IsAssignableFrom(entity.GetType()))))
                {
                    return Info.OfMethod("RomanticWeb","RomanticWeb.Entities.Entity","AsEntity").MakeGenericMethod(entityType).Invoke(entity,null);
                }
                else
                {
                    return entity;
                }
            }

            return entity;
        }

        private ILiteralNodeConverter GetBestConverter(Node literalNode)
        {
            var matches=from converter in Converters
                        let match = converter.CanConvert(literalNode)
                        where match.LiteralFormatMatches!=MatchResult.NoMatch
                           && match.DatatypeMatches!=MatchResult.NoMatch
                        orderby match
                        select converter;

            return matches.FirstOrDefault();
        }
    }
}