using System;
using System.Collections.Generic;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    [Culture("pl,en")]
    public class DecimalConverterTests : XsdConverterTestsBase<DecimalConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Decimal;
            }
        }

        [TestCase("0", 0)]
        [TestCase("-8", -8)]
        [TestCase("2.12", 2.12)]
        [TestCase("-30.555", -30.555)]
        public void Should_convert_values_from_literals(string literal, decimal expectedValue)
        {
            // when
            var value = Converter.Convert(Node.ForLiteral(literal));

            // then
            Assert.That(value, Is.InstanceOf<decimal>());
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [Test]
        [ExpectedException]
        public void Should_not_convert_scientific_notation()
        {
            Converter.Convert(Node.ForLiteral("2e10"));
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