using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class EntityMappingTests
    {
        [Test]
        public void Should_throw_if_property_is_unmapped()
        {
            // given
            var mapping = new EntityMapping(typeof(IPerson));

            // then
            Assert.Throws<MappingException>(() => mapping.PropertyFor("SomeProperty"));
        } 
    }
}