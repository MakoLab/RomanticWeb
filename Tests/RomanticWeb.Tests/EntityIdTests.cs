using System;
using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests
{
	[TestFixture]
	public class EntityIdTests
	{
		[Test]
		public void Two_instances_should_be_equal()
		{
			// given
			const string Id = "urn:test:identifier";

			// when
            var entityId1 = new EntityId(Id);
            var entityId2 = new EntityId(Id);

			Assert.That(entityId1, Is.EqualTo(entityId2));
		}

        [Test]
        public void Two_instances_should_be_equal_when_compared()
        {
            // given
            const string Id = "urn:test:identifier";

            // when
            IComparable entityId1 = new EntityId(Id);
            var entityId2 = new EntityId(Id);

            Assert.That(entityId1.CompareTo(entityId2), Is.EqualTo(0));
        }

        [Test]
        public void Blank_id_should_be_less_than_URI_id()
        {
            // given
            const string Id = "urn:test:identifier";

            // when
            IComparable entityId1 = new EntityId(Id);
            IComparable entityId2 = new BlankId(new Uri("http://blank/node"));

            Assert.That(entityId2.CompareTo(entityId1), Is.LessThan(0));
        }

        [Test]
        public void URI_id_should_be_more_than_blank_id()
        {
            // given
            const string Id = "urn:test:identifier";

            // when
            IComparable entityId1 = new EntityId(Id);
            IComparable entityId2 = new BlankId(new Uri("http://blank/node"));

            Assert.That(entityId1.CompareTo(entityId2), Is.GreaterThan(0));
        }
	}
}