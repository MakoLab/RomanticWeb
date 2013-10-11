using System;
using System.Collections.Generic;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    public class IntegerConverterTests:XsdConverterTestsBase<IntegerConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Integer;
                yield return Xsd.Int;
                yield return Xsd.Long;
                yield return Xsd.Short;
                yield return Xsd.Byte;
                yield return Xsd.NonNegativeInteger;
                yield return Xsd.NonPositiveInteger;
                yield return Xsd.NegativeInteger;
                yield return Xsd.PositiveInteger;
                yield return Xsd.UnsignedByte;
                yield return Xsd.UnsignedInt;
                yield return Xsd.UnsignedLong;
                yield return Xsd.UnsignedShort;
            }
        }

        [TestCase("0", 0)]
        [TestCase("5", 5)]
        [TestCase("-20", -20)]
        public void Should_convert_values_from_literals(string literal, long expectedValue)
        {
            // when
            var value = Converter.Convert(Node.ForLiteral(literal));

            // then
            Assert.That(value, Is.InstanceOf<long>());
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCase("2e10")]
        [TestCase("2.3")]
        [ExpectedException]
        public void Should_not_convert_decimal_numbers(string value)
        {
            Converter.Convert(Node.ForLiteral(value));
        }

        [TestCase("some text")]
        [TestCase("2010-09-05")]
        [ExpectedException]
        public void Should_not_convert_non_numbers(string literal)
        {
            Converter.Convert(Node.ForLiteral(literal));
        }
    }
}