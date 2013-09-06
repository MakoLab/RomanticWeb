using System;
using NUnit.Framework;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityIdTests
    {
        [Test]
        public void Two_instances_should_be_equal()
        {
            // given
            const string id = "urn:test:identifier";

            // when
            var entityId1 = new EntityId(id);
            var entityId2 = new EntityId(id);

            Assert.That(entityId1, Is.EqualTo(entityId2));
        }
    }
}