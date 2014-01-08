using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Statically typed converter for <see cref="EntityId"/>.</summary>
    [Export(typeof(IComplexTypeConverter))]
    public class EntityIdConverter:EntityIdConverter<EntityId>
    {
        /// <inheritdoc/>
        protected override EntityId ConvertEntityId(EntityId id)
        {
            return id;
        }
    }

    /// <summary>Generic converter for any type of entity id.</summary>
    /// <typeparam name="TEntityId">Type of the entity identifier.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic nad non-generic class")]
    public abstract class EntityIdConverter<TEntityId>:IComplexTypeConverter where TEntityId:EntityId
    {
        /// <inheritdoc />
        public object Convert(IEntity objectNode,IEntityStore entityStore,[AllowNull] IPropertyMapping predicate)
        {
            return ConvertEntityId(objectNode.Id);
        }

        /// <inheritdoc />
        public bool CanConvert(IEntity objectNode,IEntityStore entityStore,[AllowNull] IPropertyMapping predicate)
        {
            return (predicate!=null)&&(typeof(TEntityId).IsAssignableFrom(predicate.ReturnType.FindItemType()))&&(!(objectNode.Id is BlankId));
        }

        /// <inheritdoc />
        public IEnumerable<Node> ConvertBack(object obj)
        {
            yield return Node.ForUri(((TEntityId)obj).Uri);
        }

        /// <inheritdoc />
        public bool CanConvertBack(object value,IPropertyMapping predicate)
        {
            return value is TEntityId;
        }

        /// <summary>Creates a <typeparamref name="TEntityId"/> from <see cref="EntityId"/>.</summary>
        protected abstract TEntityId ConvertEntityId(EntityId id);
    }
}