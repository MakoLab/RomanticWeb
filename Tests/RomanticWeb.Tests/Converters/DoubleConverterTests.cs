using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    [Culture("pl,en")]
    public class DoubleConverterTests : XsdConverterTestsBase<DoubleConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Float;
                yield return Xsd.Double;
            }
        }

        [TestCase("+INF", double.PositiveInfinity)]
        [TestCase("INF", double.PositiveInfinity)]
        [TestCase("-INF", double.NegativeInfinity)]
        [TestCase("NaN", double.NaN)]
        [TestCase("0", 0)]
        [TestCase("2.12", 2.12d)]
        [TestCase("2e10", 2e10d)]
        public void Should_convert_values_from_literals(string literal, double expectedValue)
        {
            // when
            var value = Converter.Convert(Node.ForLiteral(literal),new Mock<IEntityContext>().Object);

            // then
            Assert.That(value, Is.InstanceOf<double>());
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}