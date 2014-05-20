using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Transforms the resulting nodes to an RDF:list adapter
    /// </summary>
    public class RdfListTransformer:SimpleTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RdfListTransformer"/> class.
        /// </summary>
        public RdfListTransformer():base(new SingleOrDefault())
        {
        }

        /// <summary>
        /// Transforms the resulting <paramref name="nodes"/> to a <see cref="IRdfListAdapter"/>
        /// </summary>
        public override object FromNodes(IEntityProxy parent,IPropertyMapping property,IEntityContext context,[AllowNull] IEnumerable<Node> nodes)
        {
            var listHead=(IEntity)base.FromNodes(parent,property,context,nodes);
            Type[] genericArguments=new Type[] { property.DeclaringType,((ICollectionMapping)property).ElementConverter.GetType(),property.ReturnType.GetGenericArguments()[0].FindItemType() };
            Type itemType=typeof(IRdfListNode<,,>).MakeGenericType(genericArguments);
            var ctor=typeof(RdfListAdapter<,,>).MakeGenericType(genericArguments).GetConstructor(
                new Type[] { typeof(IEntityContext),typeof(IEntity),itemType,typeof(OverridingGraphSelector) });

            object head;
            if (listHead==null)
            {
                head=context.GetType().GetInterfaceMap(typeof(IEntityContext))
                    .InterfaceMethods
                    .Where(item => (item.Name=="Create")&&(item.IsGenericMethodDefinition)&&(item.GetParameters().Length==1)&&(item.GetParameters()[0].ParameterType==typeof(EntityId)))
                    .Select(item => item.MakeGenericMethod(itemType))
                    .First()
                    .Invoke(context,new object[] { new EntityId(Vocabularies.Rdf.nil) });
            }
            else
            {
                head=typeof(EntityExtensions).GetMethod("AsEntity").MakeGenericMethod(itemType).Invoke(null,new object[] { listHead });
            }

            var paremeters=parent.GraphSelectionOverride??new OverridingGraphSelector(parent.Id,parent.EntityMapping,property);
            ((IEntityProxy)((IEntity)head).UnwrapProxy()).OverrideGraphSelection(paremeters);
            return ctor.Invoke(new object[] { context,parent,head,paremeters });
        }

        /// <summary>
        /// Converts a list <paramref name="value"/> to an <see cref="IRdfListAdapter"/> if necessary and return the RDF:List's head
        /// </summary>
        /// <returns>an <see cref="IEntity"/></returns>
        /// <exception cref="ArgumentException">when value is not a collection</exception>
        public override IEnumerable<Node> ToNodes(object value,IEntityProxy proxy,IPropertyMapping property,IEntityContext context)
        {
            if (!(value is IEnumerable))
            {
                throw new ArgumentException("Value must implement IEnumerable","value");
            }

            if (typeof(IRdfListAdapter<,,>).IsAssignableFromSpecificGeneric(value.GetType()))
            {
                yield return Node.FromEntityId(((IEntity)value.GetType().GetProperty("Head").GetValue(value)).Id);
            }
            else
            {
                INodeConverter converter=((ICollectionMapping)property).ElementConverter??property.Converter;
                Type[] genericArguments=new Type[] { property.DeclaringType,converter.GetType(),property.ReturnType.GetGenericArguments()[0].FindItemType() };
                var ctor=typeof(RdfListAdapter<,,>).MakeGenericType(genericArguments).GetConstructor(
                    new[] { typeof(IEntityContext),typeof(IEntity),typeof(OverridingGraphSelector) });
                var paremeters=proxy.GraphSelectionOverride??new OverridingGraphSelector(proxy.Id,proxy.EntityMapping,property);
                object rdfList=ctor.Invoke(new object[] { context,proxy,paremeters });
                System.Reflection.InterfaceMapping interfaceMapping=rdfList.GetType().GetInterfaceMap(typeof(IRdfListAdapter<,,>).MakeGenericType(genericArguments));
                var addMethodInfo=interfaceMapping.InterfaceMethods.First(item => item.Name=="Add");

                foreach (var item in (IEnumerable)value)
                {
                    addMethodInfo.Invoke(rdfList,new object[] { item });
                }

                yield return Node.FromEntityId(((IEntity)interfaceMapping.InterfaceMethods.First(item => item.Name=="get_Head").Invoke(rdfList,null)).Id);
            }
        }
    }
}