using System.Reflection;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Changes <see cref="IEntity"/> type by calling <see cref="EntityExtensions.AsEntity{TInterface}"/> method
    /// </summary>
    public class AsEntityConverter<TEntity> : INodeConverter where TEntity : IEntity
    {
        private static readonly MethodInfo AsEntityMethod = Info.OfMethod("RomanticWeb", "RomanticWeb.Entities.EntityExtensions", "AsEntity", "IEntity").MakeGenericMethod(typeof(TEntity));
        private readonly INodeConverter _entityIdConverter;

        /// <summary>Creates instance of the <see cref="IBaseUriSelectionPolicy"/>.</summary>
        public AsEntityConverter()
        {
            _entityIdConverter = new EntityIdConverter();
        }

        /// <summary>Creates instance of the <see cref="IBaseUriSelectionPolicy"/>.</summary>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy.</param>
        public AsEntityConverter(IBaseUriSelectionPolicy baseUriSelectionPolicy)
        {
            _entityIdConverter = new EntityIdConverter(baseUriSelectionPolicy);
        }

        /// <summary>
        /// Converts entity
        /// </summary>
        public object Convert(Node node, IEntityContext context)
        {
            var entityId = (EntityId)_entityIdConverter.Convert(node, context);
            return AsEntityMethod.Invoke(null, new object[] { context.Load<IEntity>(entityId) });
        }

        /// <summary>
        /// Converts an entity back to <see cref="Node" />.
        /// </summary>
        public Node ConvertBack(object obj)
        {
            return _entityIdConverter.ConvertBack(((IEntity)obj).Id);
        }
    }
}