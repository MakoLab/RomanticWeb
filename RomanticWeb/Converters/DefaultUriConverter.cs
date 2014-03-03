using System;
using System.Collections.Generic;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Generic converter for any type of entity id
    /// </summary>
    public class DefaultUriConverter:IUriNodeConverter
    {
        /// <inheritdoc />
        public object Convert(IEntity entity, [AllowNull] IPropertyMapping predicate)
        {
            return entity.Id.Uri;
        }

        /// <inheritdoc />
        public bool CanConvert(IEntity objectNode, [AllowNull] IPropertyMapping predicate)
        {
            return (predicate!=null)&&(typeof(Uri).IsAssignableFrom(predicate.ReturnType.FindItemType()))&&(!(objectNode.Id is BlankId));
        }

        /// <inheritdoc />
        public IEnumerable<Node> ConvertBack(object obj)
        {
            yield return Node.ForUri(((Uri)obj));
        }

        /// <inheritdoc />
        public bool CanConvertBack(object value,IPropertyMapping predicate)
        {
            return value is Uri;
        }
    }
}