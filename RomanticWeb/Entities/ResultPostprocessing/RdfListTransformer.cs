using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Collections;
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
        public RdfListTransformer()
            :base(new SingleOrDefault())
        {
        }

        /// <summary>
        /// Transforms the resulting <paramref name="nodes"/> to a <see cref="IRdfListAdapter"/>
        /// </summary>
        public override object FromNodes(IEntityProxy parent,IPropertyMapping property,IEntityContext context,[AllowNull] IEnumerable<Node> nodes)
        {
            var listHead=(IEntity)base.FromNodes(parent,property,context,nodes);

            var genericArguments = property.ReturnType.GetGenericArguments();

            if (typeof(IEntity).IsAssignableFrom(genericArguments.Single()))
            {
                genericArguments = new[] { typeof(IEntity) };
            }

            var ctor =
                typeof(RdfListAdapter<>).MakeGenericType(genericArguments)
                                        .GetConstructor(
                                            new[]
                                                {
                                                    typeof(IEntityContext),
                                                    typeof(IRdfListNode),
                                                    typeof(OverridingGraphSelector)
                                                });

            IRdfListNode head;
            if (listHead == null)
            {
                head=context.Create<IRdfListNode>(Vocabularies.Rdf.nil);
            }
            else
            {
                head=listHead.AsEntity<IRdfListNode>();
            }

            var paremeters=parent.GraphSelectionOverride??new OverridingGraphSelector(parent.Id,parent.EntityMapping,property);
            ((IEntityProxy)head.UnwrapProxy()).OverrideGraphSelection(paremeters);
            return ctor.Invoke(new object[] { context, head, paremeters });
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

            if (value is IRdfListAdapter)
            {
                yield return Node.FromEntityId(((IRdfListAdapter)value).Head.Id);
            }
            else
            {
                var genericArguments = property.ReturnType.GetGenericArguments();
                var ctor =
                    typeof(RdfListAdapter<>).MakeGenericType(genericArguments)
                                            .GetConstructor(new[] { typeof(IEntityContext), typeof(OverridingGraphSelector) });
                var paremeters = proxy.GraphSelectionOverride ?? new OverridingGraphSelector(proxy.Id, proxy.EntityMapping, property);
                var rdfList = (IRdfListAdapter)ctor.Invoke(new object[] { context, paremeters });

                foreach (var item in (IEnumerable)value)
                {
                    rdfList.Add(item);
                }

                yield return Node.FromEntityId(rdfList.Head.Id);
            }
        }
    }
}