using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class GlobalizationTest : IntegrationTestsBase
    {
        [Test]
        public void Should_handle_localized_literals_properly()
        {
            var person = EntityContext.Create<IPerson>(new EntityId("http://temp.uri/person"));
            person.Name = "Carl";
            EntityContext.CurrentCulture = CultureInfo.GetCultureInfo("pl");
            person.Name = "Karol";
            EntityContext.CurrentCulture = CultureInfo.GetCultureInfo("en");
            person.Name = "Charles";
            EntityContext.Commit();

            person.Name.Should().Be("Charles");
            EntityContext.CurrentCulture = CultureInfo.GetCultureInfo("pl");
            person.Name.Should().Be("Karol");
            EntityContext.CurrentCulture = CultureInfo.InvariantCulture;
            person.Name.Should().Be("Carl");
        }

        protected override void LoadTestFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}