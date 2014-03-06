using System.Linq;
using NullGuard;
using RomanticWeb.Collections;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public class RdfListTransformer:IResultTransformer
    {
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
                                                    typeof(NamedGraphSelectionParameters)
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

            var paremeters = parent.NamedGraphSelectionParameters ?? new NamedGraphSelectionParameters(parent.Id, parent.EntityMapping, property);
            ((IEntityProxy)head.UnwrapProxy()).OverrideNamedGraphSelection(paremeters);
            return ctor.Invoke(new object[] { context, head, paremeters });
        }

        public void SetTransformed(object value,IEntityStore store)
        {
            throw new System.NotImplementedException();
        }
    }
}