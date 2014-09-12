using System;
using System.Linq;
using ImpromptuInterface;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    internal class ImpromptuInterfaceCaster : IEntityCaster
    {
        private static readonly EntityMapping EntityMapping = new EntityMapping(typeof(IEntity));

        private readonly Func<Entity, IEntityMapping, IEntityProxy> _createProxy;
        private readonly IMappingsRepository _mappings;

        public ImpromptuInterfaceCaster(
            Func<Entity, IEntityMapping, IEntityProxy> proxyFactory,
            IMappingsRepository mappings)
        {
            _createProxy = proxyFactory;
            _mappings = mappings;
        }

        public T EntityAs<T>(Entity entity, Type[] types) where T : IEntity
        {
            return (T)EntityAs(entity, typeof(T), types);
        }

        private dynamic EntityAs(Entity entity, Type requested, Type[] types)
        {
            IEntityMapping mapping;
            if (types.Length == 1)
            {
                mapping = GetMapping(types[0]);
            }
            else if (types.Length == 0)
            {
                types = new[] { requested };
                mapping = GetMapping(requested);
            }
            else
            {
                mapping = new MultiMapping(types.Select(GetMapping).ToArray());
            }

            AssertEntityTypes(entity, mapping);
            return EntityAs(entity, mapping, types);
        }

        private dynamic EntityAs(Entity entity, IEntityMapping mapping, Type[] types)
        {
            var proxy = _createProxy(entity, mapping);
            return Impromptu.DynamicActLike(proxy, types);
        }

        private void AssertEntityTypes(Entity entity, IEntityMapping entityMapping)
        {
            var typed = (ITypedEntity)EntityAs(entity, _mappings.FindEntityMapping<ITypedEntity>(), new[] { typeof(ITypedEntity) });
            var currentTypes = typed.Types;
            var additionalTypes = entityMapping.Classes.Select(c => (EntityId)c.Uri);

            var entityIds = currentTypes.Union(additionalTypes).ToList();
            if (entityIds.Any())
            {
                typed.Types = entityIds;
            }
        }

        private IEntityMapping GetMapping(Type type)
        {
            if (type == typeof(IEntity))
            {
                return EntityMapping;
            }

            var mapping = _mappings.MappingFor(type);
            if (mapping == null)
            {
                throw new UnMappedTypeException(type);
            }

            return mapping;
        }
    }
}