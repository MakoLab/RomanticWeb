using System.Linq;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Transforms the resulting nodes to a <see cref="RdfListAdapter{T}"/>
    /// </summary>
    public class RdfListTransformer:IResultTransformer
    {
        /// <summary>
        /// Transforms the resulting nodes to a <see cref="RdfListAdapter{T}"/>
        /// </summary>
        public object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,[AllowNull]object value)
        {
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
            if (value==null)
            {
                head=context.Create<IRdfListNode>(Vocabularies.Rdf.nil);
            }
            else
            {
                head=((IEntity)value).AsEntity<IRdfListNode>();
            }

            var paremeters = parent.GraphSelectionOverride ?? new OverridingGraphSelector(parent.Id, parent.EntityMapping, property);
            ((IEntityProxy)head.UnwrapProxy()).OverrideGraphSelection(paremeters);
            return ctor.Invoke(new object[] { context, head, paremeters });
        }

        public void SetTransformed(object value,IEntityStore store)
        {
            throw new System.NotImplementedException();
        }
    }
}