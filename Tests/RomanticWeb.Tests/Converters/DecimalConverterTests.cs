using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    public class DecimalConverterTests : XsdConverterTestsBase<DecimalConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Decimal;
            }
        }

        [Test, Combinatorial]
        public void Should_convert_values_from_literals(
            [ValueSource("LiteralsToConvert")]Tuple<string, decimal> pair,
            [Values("en", "pl")] string culture)
        {
            using (new CultureScope(culture))
            {
                // when
                var value = Converter.Convert(Node.ForLiteral(pair.Item1), new Mock<IEntityContext>().Object);

                // then
                Assert.That(value, Is.InstanceOf<decimal>());
                Assert.That(value, Is.EqualTo(pair.Item2));
            }
        }

        [Test]
        [ExpectedException]
        public void Should_not_convert_scientific_notation()
        {
            Converter.Convert(Node.ForLiteral("2e10"), new Mock<IEntityContext>().Object);
        }

        [TestCase("some text")]
        [TestCase("2010-09-05")]
        [ExpectedException]
        public void Should_not_convert_non_numbers(string literal)
        {
            Converter.Convert(Node.ForLiteral(literal), new Mock<IEntityContext>().Object);
        }

        private static IEnumerable LiteralsToConvert()
        {
            yield return new Tuple<string, decimal>("0", 0);
            yield return new Tuple<string, decimal>("-8", -8);
            yield return new Tuple<string, decimal>("2.12", 2.12m);
            yield return new Tuple<string, decimal>("-30.555", -30.555m);
        } 
    }
}