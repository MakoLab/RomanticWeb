using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>Basic <see cref="IResultTransformer"/> which only converts nodes using <see cref="INodeConverter"/> and aggregates the result a <see cref="IResultAggregator"/>.</summary>
    [NullGuard(ValidationFlags.None)]
    public class SimpleTransformer : IResultTransformer
    {
        private readonly IResultAggregator _aggregator;

        /// <summary>Initializes a new instance of the <see cref="SimpleTransformer"/> class.</summary>
        /// <param name="aggregator">The aggregator.</param>
        public SimpleTransformer(IResultAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        /// <summary>
        /// Gets the result aggregator.
        /// </summary>
        protected IResultAggregator Aggregator { get { return _aggregator; } }

        /// <summary>Converts <paramref name="nodes"/> and returns the aggregated the result.</summary>
        public virtual object FromNodes(IEntityProxy parent, IPropertyMapping property, IEntityContext context, IEnumerable<Node> nodes)
        {
            var convertedValues = nodes.Select(node => Transform(node, property, context));
            return _aggregator.Aggregate(convertedValues);
        }

        /// <summary>Converts the given <paramref name="value"/> to <see cref="Node"/>s.</summary>
        public virtual IEnumerable<Node> ToNodes(object value, IEntityProxy proxy, IPropertyMapping property, IEntityContext context)
        {
            return new[] { property.Converter.ConvertBack(value, context) };
        }

        protected virtual object Transform(Node node, IPropertyMapping property, IEntityContext context)
        {
            object result = property.Converter.Convert(node, context);
            if (result != null)
            {
                bool isEnumerable = (property.ReturnType.IsEnumerable()) && (property.ReturnType != typeof(byte[]));
                Type itemType = property.ReturnType;
                if (isEnumerable)
                {
                    itemType = property.ReturnType.FindItemType();
                }

                if (((!isEnumerable) || (property.ReturnType != itemType)) && (!itemType.IsAssignableFrom(result.GetType())))
                {
                    if (itemType == typeof(string))
                    {
                        result = TransformToString(itemType, result);
                    }
                    else
                    {
                        result = System.Convert.ChangeType(result, itemType);
                    }
                }
            }

            return result;
        }

        private string TransformToString(Type itemType, object result)
        {
            if (typeof(IEntity).IsAssignableFrom(result.GetType()))
            {
                result = ((IEntity)result).Id.ToString(true);
            }
            else
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(result.GetType());
                if ((typeConverter != null) && (typeConverter.CanConvertTo(typeof(string))))
                {
                    result = typeConverter.ConvertToInvariantString(result);
                }
                else
                {
                    throw new InvalidOperationException(System.String.Format("Cannot transform node of type '{0}' to type of '{1}'.", result.GetType(), itemType));
                }
            }

            return (string)result;
        }
    }
}