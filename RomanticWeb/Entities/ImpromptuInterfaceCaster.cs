using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities
{
    internal class ImpromptuInterfaceCaster : IEntityCaster
    {
        private static readonly EntityMapping EntityMapping = new EntityMapping(typeof(IEntity));

        private readonly Func<Entity, IEntityMapping, IEntityProxy> _createProxy;
        private readonly IMappingsRepository _mappings;
        private readonly INamedGraphSelector _graphSelector;
        private readonly IEntityStore _store;
        private readonly IEntityMapping _typedEntityMapping;
        private readonly IPropertyMapping _typesPropertyMapping;

        public ImpromptuInterfaceCaster(
            Func<Entity, IEntityMapping, IEntityProxy> proxyFactory,
            IMappingsRepository mappings,
            [AllowNull] INamedGraphSelector graphSelector,
            IEntityStore store)
        {
            _createProxy = proxyFactory;
            _mappings = mappings;
            _graphSelector = graphSelector;
            _store = store;
            _typedEntityMapping = _mappings.MappingFor<ITypedEntity>();
            _typesPropertyMapping = _typedEntityMapping.PropertyFor("Types");
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
            Uri graphName = (_graphSelector != null ? _graphSelector.SelectGraph(entity.Id, _typedEntityMapping, _typesPropertyMapping) : null);
            var currentTypes = _store.GetObjectsForPredicate(entity.Id, RomanticWeb.Vocabularies.Rdf.type, graphName);
            var additionalTypes = entityMapping.Classes.Select(c => Model.Node.ForUri(c.Uri));

            var entityIds = currentTypes.Union(additionalTypes).ToList();
            if (entityIds.Count > 0)
            {
                _store.ReplacePredicateValues(entity.Id, Model.Node.ForUri(RomanticWeb.Vocabularies.Rdf.type), () => entityIds, graphName, entity.Context.CurrentCulture);
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