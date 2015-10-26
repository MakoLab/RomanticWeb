using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
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
            [ValueSource("ConverterTestCases")] Tuple<string, Uri, object> pair, 
            [Values("pl", "en")] string culture)
        {
            using (new CultureScope(culture))
            {
                // when
                var value = Converter.Convert(
                    (pair.Item2 != null ? Node.ForLiteral(pair.Item1, pair.Item2) : Node.ForLiteral(pair.Item1)),
                    new Mock<IEntityContext>().Object);

                // then
                Assert.That(value, Is.InstanceOf(pair.Item3.GetType()));
                Assert.That(value, Is.EqualTo(pair.Item3));
            }
        }

        private IEnumerable ConverterTestCases()
        {
            yield return new Tuple<string, Uri, object>("+INF", null, double.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("INF", null, double.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("-INF", null, double.NegativeInfinity);
            yield return new Tuple<string, Uri, object>("NaN", null, double.NaN);
            yield return new Tuple<string, Uri, object>("0", null, 0d);
            yield return new Tuple<string, Uri, object>("2.12", null, 2.12d);
            yield return new Tuple<string, Uri, object>("2e10", null, 2e10d);
            yield return new Tuple<string, Uri, object>("+INF", Xsd.Double, double.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("INF", Xsd.Double, double.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("-INF", Xsd.Double, double.NegativeInfinity);
            yield return new Tuple<string, Uri, object>("NaN", Xsd.Double, double.NaN);
            yield return new Tuple<string, Uri, object>("0", Xsd.Double, 0d);
            yield return new Tuple<string, Uri, object>("2.12", Xsd.Double, 2.12d);
            yield return new Tuple<string, Uri, object>("2e10", Xsd.Double, 2e10d);
            yield return new Tuple<string, Uri, object>("+INF", Xsd.Float, float.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("INF", Xsd.Float, float.PositiveInfinity);
            yield return new Tuple<string, Uri, object>("-INF", Xsd.Float, float.NegativeInfinity);
            yield return new Tuple<string, Uri, object>("NaN", Xsd.Float, float.NaN);
            yield return new Tuple<string, Uri, object>("0", Xsd.Float, 0f);
            yield return new Tuple<string, Uri, object>("2.12", Xsd.Float, 2.12f);
            yield return new Tuple<string, Uri, object>("2e10", Xsd.Float, 2e10f);
        } 
    }
}