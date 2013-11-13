using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    [Export(typeof(IComplexTypeConverter))]
    public class EntityIdConverter : EntityIdConverter<EntityId>
    {
        protected override EntityId ConvertEntityId(EntityId id)
        {
            return id;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic nad non-generic class")]
    public abstract class EntityIdConverter<TEntityId> : IComplexTypeConverter
        where TEntityId:EntityId
    {
        public object Convert(IEntity objectNode,IEntityStore entityStore)
        {
            return ConvertEntityId(objectNode.Id);
        }

        public bool CanConvert(IEntity objectNode,IEntityStore entityStore,[AllowNull] IPropertyMapping predicate)
        {
            return predicate!=null
                && typeof(TEntityId).IsAssignableFrom(predicate.ReturnType) 
                && !(objectNode.Id is BlankId);
        }

        public IEnumerable<Node> ConvertBack(object obj)
        {
            yield return Node.ForUri(((TEntityId)obj).Uri);
        }

        public bool CanConvertBack(object value,IPropertyMapping predicate)
        {
            return value is TEntityId;
        }

        protected abstract TEntityId ConvertEntityId(EntityId id);
    }
}