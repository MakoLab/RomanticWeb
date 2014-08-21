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

        [Test, Combinatorial]
        public void Should_convert_values_from_literals(
            [ValueSource("ConverterTestCases")] Tuple<string, double> pair, 
            [Values("pl", "en")] string culture)
        {
            using (new CultureScope(culture))
            {
                // when
                var value = Converter.Convert(Node.ForLiteral(pair.Item1), new Mock<IEntityContext>().Object);

                // then
                Assert.That(value, Is.InstanceOf<double>());
                Assert.That(value, Is.EqualTo(pair.Item2));
            }
        }

        private IEnumerable ConverterTestCases()
        {
            yield return new Tuple<string, double>("+INF", double.PositiveInfinity);
            yield return new Tuple<string, double>("INF", double.PositiveInfinity);
            yield return new Tuple<string, double>("-INF", double.NegativeInfinity);
            yield return new Tuple<string, double>("NaN", double.NaN);
            yield return new Tuple<string, double>("0", 0);
            yield return new Tuple<string, double>("2.12", 2.12d);
            yield return new Tuple<string, double>("2e10", 2e10d);
        } 
    }
}