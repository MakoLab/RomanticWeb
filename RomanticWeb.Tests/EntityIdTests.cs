using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests
{
	[TestFixture]
	public class UriIdTests
	{
		[Test]
		public void Two_instances_should_be_equal()
		{
			// given
			const string id = "urn:test:identifier";

			// when
			var entityId1 = new UriId(id);
			var entityId2 = new UriId(id);

			Assert.That(entityId1, Is.EqualTo(entityId2));
		}
	}
}