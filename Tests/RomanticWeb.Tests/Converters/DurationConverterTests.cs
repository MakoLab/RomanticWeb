using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    public class DurationConverterTests : XsdConverterTestsBase<DurationConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Duration;
            }
        }

        private IEnumerable TimeSpanValues
        {
            get
            {
                return new object[]
                           {
                               new object[] { "PT1H30M", new Duration(1, 30, 0, 0) },
                               new object[] { "-PT1H30M", new Duration(-1, 30, 0, 0) }
                           };
            }
        }

        [Test]
        [TestCaseSource("TimeSpanValues")]
        public void Should_convert_values(string literal, Duration expected)
        {
            var duration = Converter.Convert(Node.ForLiteral(literal), new Mock<IEntityContext>().Object);
            Assert.That(duration, Is.EqualTo(expected));
        }
    }
}