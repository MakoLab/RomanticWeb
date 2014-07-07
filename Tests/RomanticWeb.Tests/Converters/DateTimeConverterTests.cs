using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    public class DateTimeConverterTests : XsdConverterTestsBase<DateTimeConverter>
    {
        protected override IEnumerable<Uri> SupportedXsdTypes
        {
            get
            {
                yield return Xsd.Date;
                yield return Xsd.DateTime;
                yield return Xsd.Time;
            }
        }

        [TestCase("2010-03-23")]
        public void Should_convert_date_only(string dateValue)
        {
            // when
            var date = (DateTime)Converter.Convert(Node.ForLiteral(dateValue), new Mock<IEntityContext>().Object);

            // then
            Assert.That(date.Kind, Is.EqualTo(DateTimeKind.Unspecified));
            Assert.That(date, Is.EqualTo(new DateTime(2010, 03, 23)));
        }

        [TestCase("14:30:44")]
        public void Should_convert_time_only(string time)
        {
            // when
            var date = (DateTime)Converter.Convert(Node.ForLiteral(time), new Mock<IEntityContext>().Object);

            // then
            Assert.That(date.Kind, Is.EqualTo(DateTimeKind.Unspecified));
            Assert.That(date.TimeOfDay, Is.EqualTo(new DateTime(2010, 03, 23, 14, 30, 44).TimeOfDay));
        }

        [Test]
        public void Should_convert_date_with_time()
        {
            // when
            var date = (DateTime)Converter.Convert(Node.ForLiteral("2010-03-23T12:30:44"), new Mock<IEntityContext>().Object);

            // then
            Assert.That(date.Kind, Is.EqualTo(DateTimeKind.Unspecified));
            Assert.That(date, Is.EqualTo(new DateTime(2010, 03, 23, 12, 30, 44)));
        }

        [TestCase("2010-03-23T14:30:44+05:30", 09, 0)]
        [TestCase("2010-03-23T14:30:44Z", 14, 30)]
        [TestCase("2010-03-23T14:30:44+14:00", 00, 30)]
        public void Should_convert_date_with_time_and_timezone(string dateValue, int hour, int minutes)
        {
            // when
            var date = (DateTime)Converter.Convert(Node.ForLiteral(dateValue), new Mock<IEntityContext>().Object);

            // then
            Assert.That(date.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(date, Is.EqualTo(new DateTime(2010, 03, 23, hour, minutes, 44)));
        }

        [TestCase("14:30:44+05:30", 09, 0)]
        [TestCase("14:30:44Z", 14, 30)]
        [TestCase("14:30:44+14:00", 00, 30)]
        public void Should_convert_time_and_timezone(string dateValue, int hour, int minutes)
        {
            // when
            var date = (DateTime)Converter.Convert(Node.ForLiteral(dateValue), new Mock<IEntityContext>().Object);

            // then
            Assert.That(date.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(date.TimeOfDay, Is.EqualTo(new DateTime(2010, 03, 23, hour, minutes, 44).TimeOfDay));
        }
    }
}