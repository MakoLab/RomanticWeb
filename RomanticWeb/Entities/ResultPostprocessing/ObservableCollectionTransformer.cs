using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using ImpromptuInterface;
using NullGuard;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>Transforms RDF object values to an <see cref="ObservableCollection{T}"/>.</summary>
    public class ObservableCollectionTransformer : SimpleTransformer
    {
        private static readonly MethodInfo EnumerableCast = Info.OfMethod("System.Core", "System.Linq.Enumerable", "Cast", "IEnumerable");

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionTransformer"/> class.</summary>
        public ObservableCollectionTransformer() : base(new OriginalResult())
        {
        }

        /// <summary>Get an <see cref="ObservableCollection{T}"/> containing <paramref name="nodes"/>' values.</summary>
        public override object FromNodes(IEntityProxy parent, IPropertyMapping property, IEntityContext context, [AllowNull] IEnumerable<Node> nodes)
        {
            var convertedValues = nodes.Select(node => ((ICollectionMapping)property).ElementConverter.Convert(node, context));
            var collectionElements = ((IEnumerable<object>)Aggregator.Aggregate(convertedValues)).ToArray();

            var genericArguments = (property.ReturnType.IsArray ? new[] { property.ReturnType.GetElementType() } : property.ReturnType.GetGenericArguments());
            if (typeof(IEntity).IsAssignableFrom(genericArguments.Single()))
            {
                genericArguments = new[] { typeof(IEntity) };
            }

            var castMethod = EnumerableCast.MakeGenericMethod(genericArguments);
            var convertedCollection = castMethod.Invoke(null, new object[] { collectionElements });
            var observable = (INotifyCollectionChanged)typeof(ObservableCollection<>).MakeGenericType(genericArguments)
                .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(genericArguments) })
                .Invoke(new[] { convertedCollection });

            observable.CollectionChanged += (sender, args) => Impromptu.InvokeSet(parent, property.Name, sender);
            return observable;
        }

        /// <summary>Gets a node for each collection element.</summary>
        public override IEnumerable<Node> ToNodes(object collection, IEntityProxy proxy, IPropertyMapping property, IEntityContext context)
        {
            return from object value in (IEnumerable)collection
                   select base.ToNodes(value, proxy, property, context).Single();
        }

        protected override object Transform(Node node, IPropertyMapping property, IEntityContext context)
        {
            return property.Converter.Convert(node, context);
        }
    }
}