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
    public class EntityIdConverter : EntityIdConverter<EntityId>
    {
        /// <summary>Creates an instance of the <see cref="EntityIdConverter"/>.</summary>
        public EntityIdConverter() : base()
        {
        }

        /// <summary>Creates an instance of the <see cref="EntityIdConverter"/>.</summary>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy.</param>
        public EntityIdConverter(IBaseUriSelectionPolicy baseUriSelectionPolicy) : base(baseUriSelectionPolicy)
        {
        }
    }

    /// <summary>Generic converter for any type of entity id.</summary>
    /// <typeparam name="TEntityId">Type of the entity identifier.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic nad non-generic class")]
    public class EntityIdConverter<TEntityId> : INodeConverter where TEntityId : EntityId
    {
        private static TypeConverter _converter = TypeDescriptor.GetConverter(typeof(TEntityId));

        private IBaseUriSelectionPolicy _baseUriSelectionPolicy = null;

        /// <summary>Creates an instance of the <see cref="EntityIdConverter<TEntityId>"/>.</summary>
        public EntityIdConverter()
        {
        }

        /// <summary>Creates an instance of the <see cref="EntityIdConverter<TEntityId>"/>.</summary>
        /// <param name="baseUriSelectionPolicy">Base Uri selection policy.</param>
        public EntityIdConverter(IBaseUriSelectionPolicy baseUriSelectionPolicy)
        {
            _baseUriSelectionPolicy = baseUriSelectionPolicy;
        }

        /// <inheritdoc />
        public object Convert(Node node, IEntityContext context)
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
            return (predicate != null) && (typeof(TEntityId).IsAssignableFrom(predicate.ReturnType.FindItemType())) && (!(objectNode.Id is BlankId));
        }

        /// <inheritdoc />
        public Node ConvertBack(object obj)
        {
            if (obj is BlankId)
            {
                BlankId blank = (BlankId)obj;
                return Node.ForBlank(blank.Identifier, blank.RootEntityId, blank.Graph);
            }

            Uri uri = ((TEntityId)obj).Uri;
            if ((!uri.IsAbsoluteUri) && (_baseUriSelectionPolicy != null))
            {
                uri = new Uri(_baseUriSelectionPolicy.SelectBaseUri((TEntityId)obj), uri);
            }

            return Node.ForUri(uri);
        }
    }
}