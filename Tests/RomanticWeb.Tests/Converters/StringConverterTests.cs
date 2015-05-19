using System.Globalization;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;

namespace RomanticWeb.Tests.Converters
{
    [TestFixture]
    public class StringConverterTests
    {
        private ILiteralNodeConverter _converter;

        [TestCase("test", null)]
        [TestCase("test", "en")]
        public void Should_convert_values_back_to_literals(string literal, string language)
        {
            // given
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.SetupGet(instance => instance.CurrentCulture).Returns(() => (language != null ? CultureInfo.GetCultureInfo(language) : CultureInfo.InvariantCulture));

            // when
            var value = _converter.ConvertBack(literal, context.Object);

            // then
            Assert.That(value.IsLiteral, Is.True);
            Assert.That(value.Literal, Is.EqualTo(literal));
            Assert.That(value.Language, Is.EqualTo(language));
        }

        [SetUp]
        public void Setup()
        {
            _converter = new StringConverter();
        }
    }
}