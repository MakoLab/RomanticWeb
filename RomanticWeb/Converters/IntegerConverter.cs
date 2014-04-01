using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts XSD numeric types to numbers
    /// </summary>
    public class IntegerConverter:XsdConverterBase
    {
        private static readonly Dictionary<Type,IList<Uri>> IntegerTypes; 

        static IntegerConverter()
        {
            IntegerTypes=new Dictionary<Type,IList<Uri>>();
            IntegerTypes[typeof(int)]=new[] { Xsd.Int };
            IntegerTypes[typeof(uint)]=new[] { Xsd.UnsignedInt };
            IntegerTypes[typeof(ushort)]=new[] { Xsd.UnsignedShort };
            IntegerTypes[typeof(short)]=new[] { Xsd.Short };
            IntegerTypes[typeof(sbyte)]=new[] { Xsd.Byte };
            IntegerTypes[typeof(byte)]=new[] { Xsd.UnsignedByte };
            IntegerTypes[typeof(byte)]=new[] { Xsd.UnsignedByte };
            IntegerTypes[typeof(long)]=new[] { Xsd.Long,Xsd.Integer,Xsd.NonPositiveInteger,Xsd.NegativeInteger };
            IntegerTypes[typeof(ulong)]=new[] { Xsd.UnsignedLong,Xsd.NonNegativeInteger,Xsd.PositiveInteger };
        }

        /// <summary>
        /// Gets the supported .NET types.
        /// </summary>
        public static Type[] SupportedTypes
        {
            get
            {
                return IntegerTypes.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets xsd integral numeric datatypes
        /// </summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                return IntegerTypes.Values.SelectMany(t => t);
            }
        }
        
        /// <inheritdoc/>
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(XmlConvert.ToString((dynamic)value),IntegerTypes[value.GetType()].First());
        }

        /// <summary>
        /// Converts xsd:int (and subtypes) into <see cref="long"/>
        /// </summary>
        protected override object ConvertInternal(Node objectNode)
        {
            var returnType = typeof(Int64);
            if (objectNode.DataType != null)
            {
                returnType = IntegerTypes.Single(pair => pair.Value.Contains(objectNode.DataType, AbsoluteUriComparer.Default)).Key;
            }

            return System.Convert.ChangeType(objectNode.Literal, returnType);
        }
    }
}