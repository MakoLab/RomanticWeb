using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    public abstract class XsdConverterTestsBase<TConverter> where TConverter:ILiteralNodeConverter,new()
    {
        private TConverter _converter;

        protected TConverter Converter
        {
            get
            {
                return _converter;
            }
        }

        protected IEnumerable DatatypesSupportedByConverter
        {
            get
            {
                return from type in XsdDatatypes
                       where SupportedXsdTypes.Contains(type.Item1, new AbsoluteUriComparer())
                       select new object[] { type.Item1, type.Item2 };
            }
        }

        protected abstract IEnumerable<Uri> SupportedXsdTypes { get; }

        private static IEnumerable<Tuple<Uri,Type>> XsdDatatypes
        {
            get
            {
                yield return Tuple.Create(Xsd.Int,typeof(int));
                yield return Tuple.Create(Xsd.Long,typeof(long));
                yield return Tuple.Create(Xsd.Byte,typeof(sbyte));
                yield return Tuple.Create(Xsd.Short,typeof(short));
                yield return Tuple.Create(Xsd.Float,typeof(float));
                yield return Tuple.Create(Xsd.Integer,typeof(long));
                yield return Tuple.Create(Xsd.Double,typeof(double));
                yield return Tuple.Create(Xsd.Decimal,typeof(decimal));
                yield return Tuple.Create(Xsd.UnsignedInt,typeof(uint));
                yield return Tuple.Create(Xsd.UnsignedByte,typeof(byte));
                yield return Tuple.Create(Xsd.UnsignedLong,typeof(ulong));
                yield return Tuple.Create(Xsd.UnsignedShort,typeof(ushort));
                yield return Tuple.Create(Xsd.NegativeInteger,typeof(long));
                yield return Tuple.Create(Xsd.PositiveInteger,typeof(long));
                yield return Tuple.Create(Xsd.NonNegativeInteger,typeof(long));
                yield return Tuple.Create(Xsd.NonPositiveInteger,typeof(long));
            }
        }

        [SetUp]
        public void Setup()
        {
            _converter=new TConverter();
        }

        [TestCaseSource("DatatypesSupportedByConverter")]
        public void Should_support_converting_supported_xsd_types(Uri type, Type netType)
        {
            Assert.That(Converter.CanConvert(type));
        }
    }
}