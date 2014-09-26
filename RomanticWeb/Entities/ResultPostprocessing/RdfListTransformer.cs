using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Transforms the resulting nodes to an RDF:list adapter
    /// </summary>
    internal class RdfListTransformer : SimpleTransformer
    {
        private readonly EmitHelper _emitHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RdfListTransformer"/> class.
        /// </summary>
        public RdfListTransformer(EmitHelper emitHelper)
            : base(new SingleOrDefault())
        {
            _emitHelper = emitHelper;
        }

        /// <summary>
        /// Transforms the resulting <paramref name="nodes"/> to a <see cref="IRdfListAdapter{T}"/>
        /// </summary>
        public override object FromNodes(IEntityProxy parent, IPropertyMapping property, IEntityContext context, [AllowNull] IEnumerable<Node> nodes)
        {
            var listHead = (IEntity)base.FromNodes(parent, property, context, nodes);
            var ownerType = GetOwnerType(property);
            var nodeType = GetNodeType(property);
            var itemType = property.ReturnType.GetGenericArguments()[0].FindItemType();
            var ctor = typeof(RdfListAdapter<,,>).MakeGenericType(ownerType, nodeType, itemType)
                .GetConstructor(new[] { typeof(IEntityContext), typeof(IEntity), nodeType, typeof(OverridingGraphSelector) });

            object head;
            if (listHead == null)
            {
                head = context.GetType().GetInterfaceMap(typeof(IEntityContext))
                    .InterfaceMethods
                    .Where(item => (item.Name == "Create") && (item.IsGenericMethodDefinition) && (item.GetParameters().Length == 1) && (item.GetParameters()[0].ParameterType == typeof(EntityId)))
                    .Select(item => item.MakeGenericMethod(nodeType))
                    .First()
                    .Invoke(context, new object[] { new EntityId(Vocabularies.Rdf.nil) });
            }
            else
            {
                head = typeof(EntityExtensions).GetMethod("AsEntity").MakeGenericMethod(nodeType).Invoke(null, new object[] { listHead });
            }

            var paremeters = parent.GraphSelectionOverride ?? new OverridingGraphSelector(parent.Id, parent.EntityMapping, property);
            ((IEntityProxy)((IEntity)head).UnwrapProxy()).OverrideGraphSelection(paremeters);
            return ctor.Invoke(new[] { context, parent, head, paremeters });
        }

        /// <summary>
        /// Converts a list <paramref name="value"/> to an <see cref="IRdfListAdapter{T}"/> if necessary and return the RDF:List's head
        /// </summary>
        /// <returns>an <see cref="IEntity"/></returns>
        /// <exception cref="ArgumentException">when value is not a collection</exception>
        public override IEnumerable<Node> ToNodes(object value, IEntityProxy proxy, IPropertyMapping property, IEntityContext context)
        {
            if (!(value is IEnumerable))
            {
                throw new ArgumentException("Value must implement IEnumerable", "value");
            }

            if (typeof(IRdfListAdapter<>).IsAssignableFromSpecificGeneric(value.GetType()))
            {
                yield return Node.FromEntityId(((IEntity)value.GetType().GetProperty("Head").GetValue(value, null)).Id);
            }
            else
            {
                var nodeType = GetNodeType(property);
                var ownerType = GetOwnerType(property);
                var itemType = property.ReturnType.GetGenericArguments()[0].FindItemType();
                var ctor = typeof(RdfListAdapter<,,>).MakeGenericType(ownerType, nodeType, itemType)
                    .GetConstructor(new[] { typeof(IEntityContext), typeof(IEntity), typeof(OverridingGraphSelector) });
                var paremeters = proxy.GraphSelectionOverride ?? new OverridingGraphSelector(proxy.Id, proxy.EntityMapping, property);
                var rdfList = ctor.Invoke(new object[] { context, proxy, paremeters });
                var interfaceMapping = rdfList.GetType().GetInterfaceMap(typeof(IRdfListAdapter<>).MakeGenericType(itemType));
                var addMethodInfo = interfaceMapping.InterfaceMethods.First(item => item.Name == "Add");

                foreach (var item in (IEnumerable)value)
                {
                    addMethodInfo.Invoke(rdfList, new[] { item });
                }

                yield return Node.FromEntityId(((IEntity)interfaceMapping.InterfaceMethods.First(item => item.Name == "get_Head").Invoke(rdfList, null)).Id);
            }
        }

        private Type GetOwnerType(IPropertyMapping property)
        {
            return _emitHelper.GetBuilder().GetType(string.Format("{0}_{1}_ListOwner", property.DeclaringType.FullName, property.Name), true);
        }

        private Type GetNodeType(IPropertyMapping property)
        {
            return _emitHelper.GetBuilder().GetType(string.Format("{0}_{1}_ListNode", property.DeclaringType.FullName, property.Name), true);
        }
    }
}