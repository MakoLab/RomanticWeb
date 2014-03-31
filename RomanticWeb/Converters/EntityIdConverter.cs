using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Statically typed converter for <see cref="EntityId"/>.</summary>
    public class EntityIdConverter:EntityIdConverter<EntityId>
    {
    }

    /// <summary>Generic converter for any type of entity id.</summary>
    /// <typeparam name="TEntityId">Type of the entity identifier.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic nad non-generic class")]
    public class EntityIdConverter<TEntityId>:INodeConverter where TEntityId:EntityId
    {
        private static TypeConverter _converter=TypeDescriptor.GetConverter(typeof(TEntityId));

        /// <inheritdoc />
        public object Convert(Node node,IEntityContext context)
        {
            if (node.IsBlank)
            {
                return node.ToEntityId();
            }

            return _converter.ConvertFrom(node.Uri);
        }

        /// <inheritdoc />
        public bool CanConvert(IEntity objectNode, [AllowNull] IPropertyMapping predicate)
        {
            return (predicate!=null)&&(typeof(TEntityId).IsAssignableFrom(predicate.ReturnType.FindItemType()))&&(!(objectNode.Id is BlankId));
        }

        /// <inheritdoc />
        public Node ConvertBack(object obj)
        {
            return Node.ForUri(((TEntityId)obj).Uri);
        }
    }
}