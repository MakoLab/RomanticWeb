using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
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
                               new object[] { "PT1H30M",new TimeSpan(-1,30,0) }
                           };
            }
        } 

        [TestCaseSource("TimeSpanValues")]
        public void Should_convert_values(string literal,TimeSpan expected)
        {
            Assert.Inconclusive();

            // when
            var timeSpan=Converter.Convert(Node.ForLiteral(literal));

            // then
            Assert.That(timeSpan,Is.EqualTo(expected));
        }
    }

    ////public class TimeConverterTests:XsdConverterTestsBase<TimeConverter>
    ////{
    ////}
}