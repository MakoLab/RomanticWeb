using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    [Export(typeof(IComplexTypeConverter))]
    public class DefaultUriConverter : IComplexTypeConverter
    {
        /// <inheritdoc />
        public object Convert(IEntity objectNode,IEntityStore entityStore)
        {
            return objectNode.Id.Uri;
        }

        /// <inheritdoc />
        public bool CanConvert(IEntity objectNode,IEntityStore entityStore,[AllowNull] IPropertyMapping predicate)
        {
            return predicate!=null
                && typeof(Uri).IsAssignableFrom(predicate.ReturnType.FindItemType()) 
                && !(objectNode.Id is BlankId);
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