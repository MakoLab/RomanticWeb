using System.Reflection;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Changes <see cref="IEntity"/> type by calling <see cref="EntityExtensions.AsEntity{TInterface}"/> method
    /// </summary>
    public class AsEntityConverter<TEntity>:INodeConverter where TEntity:IEntity
    {
        private static readonly MethodInfo AsEntityMethod=Info.OfMethod("RomanticWeb","RomanticWeb.Entities.EntityExtensions","AsEntity","IEntity").MakeGenericMethod(typeof(TEntity));
        private static readonly INodeConverter EntityIdConverter=new EntityIdConverter();

        /// <summary>
        /// Converts entity
        /// </summary>
        public object Convert(Node node,IEntityContext context)
        {
            var entityId=(EntityId)EntityIdConverter.Convert(node,context);
            return AsEntityMethod.Invoke(null,new object[] { context.Load<IEntity>(entityId,false) });
        }

        /// <summary>
        /// Converts an entity back to <see cref="Node" />.
        /// </summary>
        public Node ConvertBack(object obj)
        {
            return Node.FromEntityId(((IEntity)obj).Id);
        }
    }
}