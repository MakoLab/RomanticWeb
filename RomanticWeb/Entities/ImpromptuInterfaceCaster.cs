using System;
using System.Linq;
using ImpromptuInterface;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    internal class ImpromptuInterfaceCaster : IEntityCaster
    {
        private static readonly EntityMapping EntityMapping = new EntityMapping(typeof(IEntity));
        
        private readonly IResultTransformerCatalog _transformerCatalog;
        private readonly INamedGraphSelector _graphSelector;
        private readonly IMappingsRepository _mappings;

        public ImpromptuInterfaceCaster(
            IResultTransformerCatalog transformerCatalog,
            INamedGraphSelector graphSelector, 
            IMappingsRepository mappings)
        {
            _transformerCatalog = transformerCatalog;
            _graphSelector = graphSelector;
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
            var proxy = new EntityProxy(entity, mapping, _transformerCatalog, _graphSelector);
            return Impromptu.DynamicActLike(proxy, types);
        }

        private void AssertEntityTypes(Entity entity, IEntityMapping entityMapping)
        {
            var typed = (ITypedEntity)EntityAs(entity, _mappings.FindEntityMapping<ITypedEntity>(), new[] { typeof(ITypedEntity) });
            var currentTypes = typed.Types.Select(t => t.Uri).ToArray();
            var additionalTypes = entityMapping.Classes.Select(c => c.Uri);

            foreach (var rdfType in currentTypes.Union(additionalTypes))
            {
                typed.Types.Add(rdfType);
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