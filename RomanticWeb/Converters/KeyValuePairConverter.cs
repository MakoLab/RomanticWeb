using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Generic converter for all <see cref="System.Collections.Generic.KeyValuePair&lt;TKey,TValue&gt;" /> based dictionaries.</summary>
    [Export(typeof(IComplexTypeConverter))]
    public class KeyValuePairConverter:IComplexTypeConverter
    {
        /// <summary>Converts nodes into a dictionary.</summary>
        /// <param name="objectNode">The root node of the structure</param>
        /// <param name="entityStore">Store, from which relevant additional nodes are read</param>
        /// <param name="predicate">Predicate for this node.</param>
        public virtual object Convert(IEntity objectNode, IEntityStore entityStore, [AllowNull] IPropertyMapping predicate)
        {
            dynamic asDynamic=objectNode.AsDynamic();
            return new DictionaryEntry(asDynamic.rdf.First_predicate,asDynamic.rdf.First_object);
        }

        /// <summary>Checks whether a node can be converted.</summary>
        /// <param name="objectNode">Node to be checked.</param>
        /// <param name="entityStore">Entity store.</param>
        /// <param name="predicate">Predicate for this node.</param>
        public virtual bool CanConvert(IEntity objectNode, IEntityStore entityStore, [AllowNull] IPropertyMapping predicate)
        {
            bool result=(objectNode!=null);
            if (predicate!=null)
            {
                result&=(typeof(IDictionary<,>).IsAssignableFromSpecificGeneric(predicate.ReturnType));
            }

            if (result)
            {
                dynamic asDynamic=objectNode.AsDynamic();
                return (asDynamic.rdf.Has_predicate)&&(asDynamic.rdf.Has_object);
            }

            return result;
        }

        /// <summary>Converts a value back to <see cref="Node"/>(s).</summary>
        /// <param name="obj">Object to be converted.</param>
        public virtual IEnumerable<Node> ConvertBack(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks whether a value can be converted back to <see cref="Node"/>(s).</summary>
        /// <param name="value">Value to be checked.</param>
        /// <param name="predicate">Property mapping for this value.</param>
        public virtual bool CanConvertBack(object value,IPropertyMapping predicate)
        {
            return (value!=null)&&((typeof(KeyValuePair<,>).IsAssignableFrom(value.GetType()))||(value is DictionaryEntry));
        }
    }
}