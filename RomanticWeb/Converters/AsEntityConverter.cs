using System;
using System.Collections.Generic;
using System.Reflection;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    public class AsEntityConverter:IUriNodeConverter
    {
        private static readonly MethodInfo AsEntityMethod=Info.OfMethod("RomanticWeb","RomanticWeb.Entities.EntityExtensions","AsEntity","IEntity");

        public object Convert(IEntity entity,IPropertyMapping predicate)
        {
            var itemType=predicate.ReturnType.FindItemType();
            if ((itemType!=entity.GetType())&&(!itemType.IsAssignableFrom(entity.GetType())))
            {
                return AsEntityMethod.MakeGenericMethod(itemType).Invoke(null,new object[] { entity });
            }
            else
            {
                return entity;
            }
        }

        public bool CanConvert(IEntity objectNode,[AllowNull]IPropertyMapping predicate)
        {
            return (predicate!=null)&&(PredicateIsEntityOrCollectionThereof(predicate));
        }

        public IEnumerable<Node> ConvertBack(object obj)
        {
            yield return Node.FromEntityId(((IEntity)obj).Id);
        }

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