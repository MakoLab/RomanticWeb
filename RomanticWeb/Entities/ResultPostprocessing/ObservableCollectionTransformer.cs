using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public class ObservableCollectionTransformer:IResultTransformer
    {
        public object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,[AllowNull]object value)
        {
            var genericArguments = property.ReturnType.GetGenericArguments();
            if (typeof(IEntity).IsAssignableFrom(genericArguments.Single()))
            {
                genericArguments = new[] { typeof(IEntity) };
            }

            var castMethod = Info.OfMethod("System.Core", "System.Linq.Enumerable", "Cast", "IEnumerable").MakeGenericMethod(genericArguments);

            var convertedCollection = castMethod.Invoke(null, new[] { value });
            var observable =
                (INotifyCollectionChanged)
                typeof(ObservableCollection<>).MakeGenericType(genericArguments)
                                              .GetConstructor(
                                                  new[] { typeof(IEnumerable<>).MakeGenericType(genericArguments) })
                                              .Invoke(new[] { convertedCollection });

            observable.CollectionChanged+=(sender,args) => Impromptu.InvokeSet(parent,property.Name,sender);
            return observable;
        }

        public void SetTransformed(object value,IEntityStore store)
        {
            throw new System.NotImplementedException();
        }
    }
}