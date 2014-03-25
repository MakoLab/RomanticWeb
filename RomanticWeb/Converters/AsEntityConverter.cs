using System;
using System.Collections.Generic;
using System.Reflection;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Changes <see cref="IEntity"/> type by calling <see cref="EntityExtensions.AsEntity{TInterface}"/> method
    /// </summary>
    public class AsEntityConverter:IUriNodeConverter
    {
        private static readonly MethodInfo AsEntityMethod=Info.OfMethod("RomanticWeb","RomanticWeb.Entities.EntityExtensions","AsEntity","IEntity");

        /// <summary>
        /// Converts entity
        /// </summary>
        public object Convert(IEntity entity,IPropertyMapping predicate)
        {
            var itemType=predicate.ReturnType.FindItemType();
            if ((itemType!=entity.GetType())&&(!itemType.IsInstanceOfType(entity)))
            {
                return AsEntityMethod.MakeGenericMethod(itemType).Invoke(null,new object[] { entity });
            }
            
            return entity;
        }

        /// <summary>
        /// Checks whether an entity can be converted.
        /// </summary>
        public bool CanConvert(IEntity objectNode,[AllowNull]IPropertyMapping predicate)
        {
            return (predicate!=null)&&(PredicateIsEntityOrCollectionThereof(predicate));
        }

        /// <summary>
        /// Converts an entity back to <see cref="Node" />.
        /// </summary>
        public IEnumerable<Node> ConvertBack(object obj)
        {
            yield return Node.FromEntityId(((IEntity)obj).Id);
        }

        /// <summary>
        /// Checks whether an entity can be converted back to <see cref="Node" />(s).
        /// </summary>
        public bool CanConvertBack(object value,[AllowNull]IPropertyMapping predicate)
        {
            return (predicate!=null)&&(value is IEntity);
        }

        private static bool PredicateIsEntityOrCollectionThereof(IPropertyMapping predicate)
        {
            if (predicate == null)
            {
                return false;
            }

            if (typeof(IEntity).IsAssignableFrom(predicate.ReturnType))
            {
                return true;
            }

            if (typeof(IEnumerable<IEntity>).IsAssignableFrom(predicate.ReturnType))
            {
                return true;
            }

            return false;
        }
    }
}