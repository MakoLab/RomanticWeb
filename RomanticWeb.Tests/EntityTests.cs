using System;
using NUnit.Framework;

namespace RomanticWeb.Tests
{
	[TestFixture]
	public class EntityTests
	{
		[Test]
		public void Creating_with_new_should_allow_setting_properties()
		{
			// given
			var entity = new Entity(new UriId(string.Format("urn:uuid:{0}", Guid.NewGuid())));
			var testEntity = entity.AsEntity<ITestEntity>();

			// when
			testEntity.Name = "Tomasz";

			// then
			Assert.That(testEntity.Name, Is.EqualTo("Tomasz"));
		}

		[Test]
		public void Entity_should_hold_separate_values_for_interfaces()
		{
			// given
			var entity = new Entity(new UriId(string.Format("urn:uuid:{0}", Guid.NewGuid())));
			var testEntity = entity.AsEntity<ITestEntity>();
			testEntity.Name = "Tomasz";

			// when
			var otherEntity = entity.AsEntity<IOtherEntity>();

			// then
			Assert.That(otherEntity.Name, Is.Null);
		}

		public interface ITestEntity:IEntity
		{
			string Name { get; set; }
		}

		public interface IOtherEntity:IEntity
		{
			string Name { get; set; }
		}
	}
}